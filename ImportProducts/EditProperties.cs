﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImportProducts
{
    public partial class EditProperties : Form
    {
        public EditProperties()
        {
            InitializeComponent();
            SelectedHotelsEntities db = new SelectedHotelsEntities();
            List<Location> countries = (from l in db.Locations
                                        where l.ParentId == null
                                       orderby l.Name
                                       select l).ToList();
            Location emptyCountry = new Location { Id = 0, Name = String.Empty };
            countries.Insert(0, emptyCountry);
            comboBoxCountry.DataSource = countries;
        }

        private void textBoxURL_Validating(object sender, CancelEventArgs e)
        {
            if (textBoxURL.Text.Length > 0)
            {
                errorProvider1.SetError(textBoxURL, "");
            }
            else
            {
                errorProvider1.SetError(textBoxURL, "Please enter URL");
            }
        }

        private void textBoxCategory_Validating(object sender, CancelEventArgs e)
        {
            if (textBoxCategory.Text.Length > 0)
            {
                errorProvider1.SetError(textBoxCategory, "");
            }
            else
            {
                errorProvider1.SetError(textBoxCategory, "Please enter Category");
            }
        }

        private void textBoxAdvancedCategoryRoot_Validating(object sender, CancelEventArgs e)
        {
            if (textBoxAdvancedCategoryRoot.Text.Length > 0)
            {
                errorProvider1.SetError(textBoxAdvancedCategoryRoot, "");
            }
            else
            {
                errorProvider1.SetError(textBoxAdvancedCategoryRoot, "Please enter Advanced Category Root");
            }
        }

        private void comboBoxCountry_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
