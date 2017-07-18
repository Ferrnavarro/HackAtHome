using Android.App;
using Android.Widget;
using Android.OS;
using HackAtHome.SAL;
using HackAtHome.Entities;

namespace HackAtHomeClient
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
             SetContentView (Resource.Layout.Main);

            var validateButton = FindViewById<Button>(Resource.Id.button1);

            validateButton.Click += (s, ev) =>
            {
                Validate();
            };

        }


        private async void Validate()
        {
            ServiceClient serviceclient = new ServiceClient();
            var emailEditText = FindViewById<EditText>(Resource.Id.editTextCorreo);
            var passwordEditText = FindViewById<EditText>(Resource.Id.editTextPassword);



            ResultInfo Result = await serviceclient.AutenticateAsync(emailEditText.Text, passwordEditText.Text);

            var MicrosoftEvidence = new LabItem
            {
                Email = emailEditText.Text, Lab = "Hack@Home", DeviceId = Android.Provider.Settings.Secure.GetString(
                    ContentResolver, Android.Provider.Settings.Secure.AndroidId)

        };
            var MicrosoftClient = new MicrosoftServiceClient();
            await MicrosoftClient.SendEvidence(MicrosoftEvidence);

            Android.App.AlertDialog.Builder Builder = new AlertDialog.Builder(this);
            AlertDialog Alert = Builder.Create();
            Alert.SetTitle("Resultado de la verificacion");
            Alert.SetIcon(Resource.Drawable.icon);


            string message;

            if (Result.Status == Status.Success || Result.Status== Status.AllSuccess)
            {
                message = $"Bienvenido {Result.FullName}";
            }

            else
            {
                message = $"Error:\n{Result.Status}\n{Result.FullName}";
            }

          
            var Intent = new Android.Content.Intent(this, typeof(EvidenceListActivity));
            Intent.PutExtra("FullName", Result.FullName);
            Intent.PutExtra("Token", Result.Token);
            StartActivity(Intent);
        }


    }
}

