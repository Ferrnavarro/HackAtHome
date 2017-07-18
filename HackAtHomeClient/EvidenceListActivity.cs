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
using HackAtHome.CustomAdapters;
using HackAtHome.SAL;
using HackAtHomeClient.Fragments;

namespace HackAtHomeClient
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon")]
    public class EvidenceListActivity : Activity
    {
        private EvidenceFragment Data;
        private ListView EvidenceListView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.EvidenceList);

            
            Data = (EvidenceFragment)this.FragmentManager.FindFragmentByTag("Data");
            if (Data == null)
            {
        
                Data = new EvidenceFragment();
                var FragmentTransaction = this.FragmentManager.BeginTransaction();
                FragmentTransaction.Add(Data, "Data");
                FragmentTransaction.Commit();
            }

            LoadData();
        }
        protected override void OnPause()
        {
            
            EvidenceListView = FindViewById<ListView>(Resource.Id.listViewEvidence);
            Data.EvidenceListState = EvidenceListView.OnSaveInstanceState();
            base.OnPause();
        }

        
        private async void LoadData()
        {
            
            if (string.IsNullOrWhiteSpace(Data.Token))
            {
                
                Data.FullName = Intent.GetStringExtra("FullName");
                Data.Token = Intent.GetStringExtra("Token");

               
                var ServiceClient = new ServiceClient();

                try
                {
                    Data.EvidenceList = await ServiceClient.GetEvidencesAsync(Data.Token);
                }
                catch (Exception ex)
                {
                   
                    ErrorDialog(ex.Message);
                    Data.EvidenceList = new List<HackAtHome.Entities.Evidence>();
                }
            }

           
            var FullNameTextView = FindViewById<TextView>(Resource.Id.textViewFullName);
            FullNameTextView.Text = Data.FullName;

         
            var EvidenceAdapter = new EvidencesAdapter(
                this, Data.EvidenceList, Resource.Layout.EvidenceListItem,
                Resource.Id.textViewEvidenceTitle, Resource.Id.textViewEvidenceStatus
            );
            


            EvidenceListView = FindViewById<ListView>(Resource.Id.listViewEvidence);
            EvidenceListView.Adapter = EvidenceAdapter;

            
            if (Data.EvidenceListState != null)
            {
                EvidenceListView.OnRestoreInstanceState(Data.EvidenceListState);
            }

           
            EvidenceListView.ItemClick += (s, ev) =>
            {
                var Intent = new Android.Content.Intent(this, typeof(EvidenceDetailActivity));
                Intent.PutExtra("FullName", Data.FullName);
                Intent.PutExtra("Token", Data.Token);

                var Evidence = EvidenceAdapter[ev.Position];
                Intent.PutExtra("EvidenceID", Evidence.EvidenceID);
                Intent.PutExtra("EvidenceTitle", Evidence.Title);
                Intent.PutExtra("EvidenceStatus", Evidence.Status);
                StartActivity(Intent);
            };
        }

        /// <summary>
        /// Genera un dialog de error para la aplicacion
        /// </summary>
        /// <param name="title">Titulo a proporicionar </param>
        /// <param name="iconResource">Icono del dialogo</param>
        /// <param name="message">Mensaje a mostrar</param>
        /// 
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