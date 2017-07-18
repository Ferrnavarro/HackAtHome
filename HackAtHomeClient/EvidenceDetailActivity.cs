using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HackAtHome.SAL;
using HackAtHomeClient.Fragments;
using Android.Webkit;
using Android.Graphics;

namespace HackAtHomeClient
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon")]
    public class EvidenceDetailActivity : Activity
    {
        private EvidenceDetailFragment Data;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EvidenceDetail);

           
            Data = (EvidenceDetailFragment)this.FragmentManager.FindFragmentByTag("Data");
            if (Data == null)
            {
                
                Data = new EvidenceDetailFragment();
                var FragmentTransaction = this.FragmentManager.BeginTransaction();
                FragmentTransaction.Add(Data, "Data");
                FragmentTransaction.Commit();
            }

            LoadData();
        }

        /// <summary>
        /// Cargamos la informacion en Data para no perder la informacion.
        /// </summary>
        private async void LoadData()
        {
            
            if (Data.Evidence == null)
            {
              
                Data.FullName = Intent.GetStringExtra("FullName");
                Data.Token = Intent.GetStringExtra("Token");

                Data.Evidence = new HackAtHome.Entities.Evidence()
                {
                    EvidenceID = Intent.GetIntExtra("EvidenceID", 0),
                    Title = Intent.GetStringExtra("EvidenceTitle"),
                    Status = Intent.GetStringExtra("EvidenceStatus")
                };

                
                var serviceClient = new ServiceClient();

                try
                {
                    Data.Detail = await serviceClient.GetEvidenceByIDAsync(Data.Token, Data.Evidence.EvidenceID);
                }
                catch (Exception ex)
                {
                    
                    ErrorDialog(ex.Message);
                    Data.Detail = new HackAtHome.Entities.EvidenceDetail();
                }
            }

          
            var FullNameTextView = FindViewById<TextView>(Resource.Id.tvDetailFullName);
            FullNameTextView.Text = Data.FullName;

            var TitleTextView = FindViewById<TextView>(Resource.Id.tvDetailTitle);
            TitleTextView.Text = Data.Evidence.Title;

            var StatusTextView = FindViewById<TextView>(Resource.Id.tvDetailStatus);
            StatusTextView.Text = Data.Evidence.Status;

            var DescriptionWebView = FindViewById<WebView>(Resource.Id.webViewContent);
            DescriptionWebView.LoadDataWithBaseURL(null, $"<html><head><style type='text/css'>body{{color:#FFFFFF}}</style></head><body> {Data.Detail.Description}</body></html>", "text/html", "utf-8", null);
            
            DescriptionWebView.SetBackgroundColor(Color.Transparent);
            var EvidenceImageView = FindViewById<ImageView>(Resource.Id.imageViewEvidence);
            Koush.UrlImageViewHelper.SetUrlDrawable(EvidenceImageView, Data.Detail.Url);
        }

        /// <summary>
        /// Genera un dialog de error para la aplicacion
        /// </summary>
        /// <param name="title">Titulo a proporicionar </param>
        /// <param name="iconResource">Icono del dialogo</param>
        /// <param name="message">Mensaje a mostrar</param>
        private void ErrorDialog(string title = "Error", int iconResource = Resource.Drawable.icon, string message = "Ocurrio un error inesperado")
        {
            Android.App.AlertDialog.Builder Builder = new AlertDialog.Builder(this);
            AlertDialog Alert = Builder.Create();
            Alert.SetTitle(title);
            Alert.SetIcon(iconResource);
            Alert.SetMessage($"Ocurrio un error inesperado:\n{message}");
            Alert.SetButton("Ok", (s, ev) => { });
            Alert.Show();
        }
    }
}