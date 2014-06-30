﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using SelectedHotelsModel;

namespace ImportProducts
{
    class ImportTradeDoublerHotels
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static IEnumerable<XElement> StreamRootChildDoc(string uri)
        {
            using (XmlReader reader = XmlReader.Create(uri))
            {
                reader.MoveToContent();
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "product")
                            {
                                XElement el = XElement.ReadFrom(reader) as XElement;
                                if (el != null)
                                    yield return el;
                            }
                            break;
                    }
                }
            }
        }

        public static void SaveFileFromURL(string url, string destinationFileName, int timeoutInSeconds, BackgroundWorker bw, DoWorkEventArgs e)
        {
            // Create a web request to the URL
            HttpWebRequest MyRequest = (HttpWebRequest)WebRequest.Create(url);
            MyRequest.Timeout = timeoutInSeconds * 1000;
            try
            {
                // Get the web response
                HttpWebResponse MyResponse = (HttpWebResponse)MyRequest.GetResponse();

                // Make sure the response is valid
                if (HttpStatusCode.OK == MyResponse.StatusCode)
                {
                    // Open the response stream
                    using (Stream MyResponseStream = MyResponse.GetResponseStream())
                    {
                        // Set step for backgroundWorker
                        Form1.activeStep = "Load file..";
                        bw.ReportProgress(0);           // start new step of background process
                        
                        // Open the destination file
                        using (
                            FileStream MyFileStream = new FileStream(destinationFileName, FileMode.OpenOrCreate,
                                                                     FileAccess.Write))
                        {
                            // Get size of stream - it is impossible in advance at all
                            // so we just can set approximate value if know it or get it before by experience
                            long countBuffer = 1000000;
                            long currentBuffer = 0;
                            // Create a 4K buffer to chunk the file
                            byte[] MyBuffer = new byte[4096];
                            int BytesRead;
                            // Read the chunk of the web response into the buffer
                            while (0 < (BytesRead = MyResponseStream.Read(MyBuffer, 0, MyBuffer.Length)))
                            {
                                // Write the chunk from the buffer to the file
                                MyFileStream.Write(MyBuffer, 0, BytesRead);
                                // show progress & catch Cancel
                                currentBuffer++;
                                if (bw.CancellationPending)
                                {
                                    // cancel background work
                                    e.Cancel = true;                                    
                                    // Due to too huge size of download it is neccessary explicit closing Stream else operation in background will be cancelled after total download file
                                    MyRequest.Abort();
                                    break;
                                }
                                else if (bw.WorkerReportsProgress && currentBuffer % 100 == 0) bw.ReportProgress((int)(100 * currentBuffer / countBuffer));
                            }
                            // visualization finish process
                            if (!e.Cancel && currentBuffer < countBuffer)
                            {
                                bw.ReportProgress(100);
                                Thread.Sleep(100);
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                e.Result = "ERROR:" + err.Message;
                log.Error("Error error logging", err);
            }
        }

        public static void DoImport(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            // Parse list input parameters
            BackgroundWorkParameters param = (BackgroundWorkParameters)e.Argument;
            string _URL = param.Url;
            int categoryId = param.CategoryId;
            int portalId = param.PortalId;
            int vendorId = param.VendorId;
            string countryFilter = param.CountryFilter;
            string cityFilter = param.CityFilter;
            int? stepImport = param.StepImport;
            int feedId = param.FeedId;

            if (!File.Exists(_URL))
            {
                string xmlFileName = String.Format("{0}\\{1}", Properties.Settings.Default.TempPath, "tradedoubler.xml");
                if (File.Exists(xmlFileName))
                {
                    File.Delete(xmlFileName);
                }
                // inside function display progressbar
                SaveFileFromURL(_URL, xmlFileName, 60, bw, e);

                // exit if user cancel during saving file or error
                if (e.Cancel || (e.Result != null) && e.Result.ToString().Substring(0, 6).Equals("ERROR:")) return;

                _URL = String.Format("{0}\\{1}", Properties.Settings.Default.TempPath, "tradedoubler.xml");
            }

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", "tradedoublerHotels.xsd");
            Form1.activeStep = "Validating input..";
            bw.ReportProgress(0); // start new step of background process
            XDocument xDoc = XDocument.Load(_URL);
            bool errors = false;
            xDoc.Validate(schemas, (o, e2) =>
            {
                e.Result = "ERROR:" + e2.Message;
                log.Error(e2.Message);
                errors = true;
            });
            if (errors)
            {
                e.Cancel = true;
                return;
            }

            // show progress & catch Cancel
            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            else if (bw.WorkerReportsProgress) bw.ReportProgress(50);

            // read hotels from XML
            // use XmlReader to avoid huge file size dependence
            var xmlProducts =
                from el in StreamRootChildDoc(_URL)
                select new
                {
                    Category = (string)el.Element("TDCategoryName"),
                    ProductNumber = (string)el.Element("TDProductId"),
                    Name = (string)el.Element("name"),
                    Image = (string)el.Element("imageUrl"),
                    UnitCost = (decimal)el.Element("price"),
                    Description = (string)el.Element("description"),
                    DescriptionHTML = (string)el.Element("description"),
                    URL = (string)el.Element("productUrl"),
                    Country = (string)el.Element("fields").Element("country"),
                    City = (string)el.Element("fields").Element("city"),
                    StarRating = (string)el.Element("fields").Element("StarRating"),
                    AverageOverallRating = (string)el.Element("fields").Element("AverageOverallRating"),
                    Address = (string)el.Element("fields").Element("address"),
                    Currency = (string)el.Element("currency"),
                    Regions1 = (string)el.Element("fields").Element("regions1"),
                    Regions2 = (string)el.Element("fields").Element("regions2"),
                    PostalCode = (string)el.Element("fields").Element("postalcode")
                };

            if (!String.IsNullOrEmpty(countryFilter))
            {
                xmlProducts = xmlProducts.Where(p => p.Regions1 == countryFilter || p.Regions2 == countryFilter);
            }

            if (!String.IsNullOrEmpty(cityFilter))
            {
                xmlProducts = xmlProducts.Where(p => p.City == cityFilter);
            }

            // show progress & catch Cancel
            if (bw.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            else if (bw.WorkerReportsProgress) bw.ReportProgress(100);
            Thread.Sleep(100); // a little bit slow working for visualisation Progress

            // Set step for backgroundWorker
            Form1.activeStep = "Import records..";
            bw.ReportProgress(0); // start new step of background process
            int productCount = xmlProducts.Count();
            try
            {
                int initialStep = 0;
                if (stepImport.HasValue)
                {
                    initialStep = stepImport.Value;
                }

                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    int i = 0;
                    foreach (var product in xmlProducts)
                    {
                        if (i < initialStep)
                        {
                            i++;
                            continue;
                        }

                        string productName = product.Name.Replace("&apos;", "'");
#if DEBUG
                        Console.WriteLine(i + " - " + productName); // debug print
#endif

                        Hotel hotel =
                            db.Products.OfType<Hotel>().SingleOrDefault(
                                p => p.Name == productName && p.Number == product.ProductNumber);
                        if (hotel == null)
                        {
                            hotel = new Hotel
                            {
                                Name = productName,
                                ProductTypeId = (int)Enums.ProductTypeEnum.Hotels,
                                Number = product.ProductNumber,
                                UnitCost = product.UnitCost,
                                Description = product.Description,
                                URL = product.URL,
                                Image = product.Image,
                                CreatedByUser = vendorId,
                                CreatedDate = DateTime.Now,
                                IsDeleted = false,
                                HotelTypeId = (int)Enums.HotelTypeEnum.Hotels
                            };

                            if (!String.IsNullOrEmpty(product.StarRating))
                            {
                                hotel.Star = decimal.Parse(product.StarRating);
                            }
                            if (!String.IsNullOrEmpty(product.AverageOverallRating))
                            {
                                hotel.CustomerRating = decimal.Parse(product.AverageOverallRating);
                            }
                            if (!String.IsNullOrEmpty(product.Address))
                            {
                                hotel.Address = product.Address;
                            }
                            if (!String.IsNullOrEmpty(product.Currency))
                            {
                                hotel.CurrencyCode = product.Currency;
                            }
                            if (!String.IsNullOrEmpty(product.PostalCode))
                            {
                                hotel.PostCode = product.PostalCode;
                            }
                            hotel.FeedId = feedId;
                            db.Products.Add(hotel);
                            db.SaveChanges();

                            Location location = null;
                            if ((product.Regions2 ?? product.Regions1) != String.Empty)
                            {
                                location = Common.AddLocation(db, product.Regions2 ?? product.Regions1, null, 1);
                                Common.SetLocation(db, location, hotel);
                            }
                            if (product.Regions1 != null && product.Regions2 != null)
                            {
                                location = Common.AddLocation(db, product.Regions1, location.Id, 2);
                                Common.SetLocation(db, location, hotel);
                            }
                            if (product.City != null)
                            {
                                location = Common.AddLocation(db, product.City, location.Id, 3);
                                Common.SetLocation(db, location, hotel);
                            }

                            Category category = db.Categories.Find(categoryId);
                            if (category != null)
                            {
                                hotel.Categories.Add(category);
                            }
                            db.SaveChanges();

                            i++;
                            Common.UpdateSteps(stepImport: i);
                        }
                        else
                        {
                            // no need to check for null vallue because of previous if
                            if (hotel.UnitCost != product.UnitCost)
                            {
                                hotel.UnitCost = product.UnitCost;
                            }
                            if (hotel.Description != product.Description)
                            {
                                hotel.Description = product.Description;
                            }
                            if (hotel.URL != product.URL)
                            {
                                hotel.URL = product.URL;
                            }
                            if (hotel.Image != product.Image)
                            {
                                hotel.Image = product.Image;
                            }
                            decimal? star = null;
                            if (!String.IsNullOrEmpty(product.StarRating))
                            {
                                star = decimal.Parse(product.StarRating);
                            }
                            if (hotel.Star != star)
                            {
                                hotel.Star = star;
                            }
                            decimal? customerRating = null;
                            if (!String.IsNullOrEmpty(product.AverageOverallRating))
                            {
                                customerRating = decimal.Parse(product.AverageOverallRating);
                            }
                            if (hotel.CustomerRating != customerRating)
                            {
                                hotel.CustomerRating = customerRating;
                            }
                            if (hotel.Address != product.Address)
                            {
                                hotel.Address = product.Address;
                            }
                            if (hotel.CurrencyCode != product.Currency)
                            {
                                hotel.CurrencyCode = product.Currency;
                            }
                            if (hotel.PostCode != product.PostalCode)
                            {
                                hotel.PostCode = product.PostalCode;
                            }
                            db.SaveChanges();

                            i++;
                            Common.UpdateSteps(stepImport: i);
                        }

                        if (bw.CancellationPending)
                        {
                            e.Cancel = true;
                            goto Cancelled;
                        }
                        else if (bw.WorkerReportsProgress && i % 100 == 0)
                        {
                            bw.ReportProgress((int) (100*i/productCount));
                        }
                    }
                }

                if (!e.Cancel)
                {
                    Common.UpdateSteps();
                }
            Cancelled:
                ;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName,
                                               validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                e.Result = "ERROR:" + ex.Message;
                log.Error("Error error logging", ex);
            }
        }
    }
}
