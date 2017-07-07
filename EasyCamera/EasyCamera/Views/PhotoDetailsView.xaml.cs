using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace EasyCamera.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PhotoDetailsView : ContentPage
    {
        public PhotoDetailsView()
        {
            InitializeComponent();

            photoName.Text = "Dave's Awesome Photo.jpg";
        }

        private void recordName_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync(true);
        }
    }
}