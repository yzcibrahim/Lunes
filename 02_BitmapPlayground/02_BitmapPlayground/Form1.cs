using _02_BitmapPlayground.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _02_BitmapPlayground
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            PopulateFilterPicker();
        }

        private void PopulateFilterPicker()
        {
            Assembly assembly = Assembly.LoadFrom("BitmapFilters.dll");

            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsInterface)
                {
                    if (type.GetInterfaces().Contains(typeof(IFilter)))
                    {
                        IFilter filter = (IFilter)Activator.CreateInstance(type);
                        FilterPickerBox.Items.Add(filter);
                    }
                }
            }

            //FilterPickerBox.Items.Add(new RedFilter());
            //FilterPickerBox.Items.Add(new GrayscaleFilter());
            //FilterPickerBox.Items.Add(new MovingAvarageFilter()); 
        }
        
        /// <summary>
        /// Applies a filter to an image.
        /// </summary>
        /// <param name="filter">The filter to apply. Must not be null.</param>
        /// <param name="image">The image to which the filter is applied.</param>
        /// <returns>A new image with the filter applied.</returns>
        private Image ApplyFilter(IFilter filter, Image image)
        {
            // Sanity check input
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            if (image == null)
                throw new ArgumentNullException(nameof(image));

            // Create a new bitmap from the existing image
            Bitmap result = new Bitmap(image);

            // Copy the pixel colors of the existing bitmap to a new 2D - color array for further processing.
            Color[,] colors = new Color[result.Width, result.Height];
            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                    colors[x, y] = result.GetPixel(x, y);

            // Apply the user defined filter.
            var filteredImageData = filter.Apply(colors);

            // Copy the resulting array back to the bitmap
            for (int x = 0; x < result.Width; x++)
                for (int y = 0; y < result.Height; y++)
                    result.SetPixel(x, y, filteredImageData[x, y]);

            return result;
        }

        private void ApplyFilterButton_Click(object sender, EventArgs e)
        {
            if (FilterPickerBox.SelectedItem is IFilter selectedFilter)
                PictureBoxFiltered.Image = ApplyFilter(selectedFilter, PictureBoxOriginal.Image);
        }
    }
}
