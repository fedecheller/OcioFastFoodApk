using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Android.OS;
using OcioFastFood.Views;
using OcioFastFood.Models;
using Android.Provider;

namespace OcioFastFood {
    [Activity(Label = "Ocio fast food", MainLauncher = true)]
    public class MainActivity : Activity {
        public override void OnBackPressed() {

            //search the ocio contact
            try {
                Android.Net.Uri uri = Android.Provider.ContactsContract.Contacts.ContentUri;
                string[] projection = { Android.Provider.ContactsContract.Contacts.InterfaceConsts.DisplayName };
                string nam = "Ocio fast food";
                String selection = string.Format("{0} = '{1}'", Android.Provider.ContactsContract.ContactsColumns.DisplayName, nam);

                var cursor = ContentResolver.Query(uri, projection, selection, null, null);
                //var cursor = ManagedQuery(uri, projection, selection, null, null);
                if (cursor.Count <= 0) {
                    contactAskDialog();
                } else {
                    Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
                }
            } catch (Exception ex) {
                Android.Util.Log.Error("Add contact", "Error on contact creation. " + ex.Message);
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
        }

        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            RequestWindowFeature(WindowFeatures.NoTitle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var webView = FindViewById<WebView>(Resource.Id.webView);
            webView.Settings.JavaScriptEnabled = true;

            // Use subclassed WebViewClient to intercept hybrid native calls
            webView.SetWebViewClient(new WebViewClient());

            webView.LoadUrl("https://www.ociofastfood.it");

        }

        private void contactAskDialog() {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("OCIO");
            alert.SetMessage("Vuoi aggiungere Ocio ai contatti?");
            alert.SetIcon(Resource.Drawable.Icon);
            alert.SetButton("Si", (c, ev) => {
                createContact();
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            });
            alert.SetButton2("No", (c, ev) => {
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            });
            alert.Show();
        }

        private void createContact() {
            System.Collections.Generic.List<ContentProviderOperation> ops = new System.Collections.Generic.List<ContentProviderOperation>();

            ContentProviderOperation.Builder builder = ContentProviderOperation.NewInsert(ContactsContract.RawContacts.ContentUri);
            builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountType, null);
            builder.WithValue(ContactsContract.RawContacts.InterfaceConsts.AccountName, null);
            ops.Add(builder.Build());

            //Name
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.StructuredName.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.FamilyName, "fast food");
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.GivenName, "Ocio");
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredName.DisplayName, "Ocio");
            ops.Add(builder.Build());

            //Number
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Phone.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.Phone.Number, "0125280747");
            builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Type, ContactsContract.CommonDataKinds.Phone.InterfaceConsts.TypeCustom);
            builder.WithValue(ContactsContract.CommonDataKinds.Phone.InterfaceConsts.Label, "Prenotazioni");
            ops.Add(builder.Build());

            //Photo
            Android.Graphics.Bitmap bitmap = Android.Graphics.BitmapFactory.DecodeResource(Resources, Resource.Drawable.Icon);
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, stream);
            byte[] data = stream.ToArray();
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Photo.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.Photo.InterfaceConsts.Data15, data);
            ops.Add(builder.Build());

            //Website
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Website.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.Website.Url, "https://www.facebook.com/ocioivrea/");
            builder.WithValue(ContactsContract.CommonDataKinds.Website.InterfaceConsts.Type, ContactsContract.CommonDataKinds.Website.InterfaceConsts.TypeCustom);
            builder.WithValue(ContactsContract.CommonDataKinds.Website.InterfaceConsts.Label, "Sito web");
            ops.Add(builder.Build());

            //Address
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.StructuredPostal.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredPostal.City, "Ivrea");
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredPostal.Country, "Italia");
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredPostal.Postcode, "10015");
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredPostal.Street, "Corso Nigra 67");
            builder.WithValue(ContactsContract.CommonDataKinds.StructuredPostal.Region, "TO");
            builder.WithValue(ContactsContract.CommonDataKinds.Website.InterfaceConsts.Type, ContactsContract.CommonDataKinds.StructuredPostal.InterfaceConsts.TypeCustom);
            builder.WithValue(ContactsContract.CommonDataKinds.Website.InterfaceConsts.Label, "Indirizzo");
            ops.Add(builder.Build());

            //Email
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Email.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Data, "info@ociofastfood.it");
            builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Type, ContactsContract.CommonDataKinds.Email.InterfaceConsts.TypeCustom);
            builder.WithValue(ContactsContract.CommonDataKinds.Email.InterfaceConsts.Label, "Email");
            ops.Add(builder.Build());

            //Company
            /*
            builder = ContentProviderOperation.NewInsert(ContactsContract.Data.ContentUri);
            builder.WithValueBackReference(ContactsContract.Data.InterfaceConsts.RawContactId, 0);
            builder.WithValue(ContactsContract.Data.InterfaceConsts.Mimetype, ContactsContract.CommonDataKinds.Organization.ContentItemType);
            builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Data, lkw + " (" + fromCountry + ">" + toCountry + ")");
            builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Type, ContactsContract.CommonDataKinds.Organization.InterfaceConsts.TypeCustom);
            builder.WithValue(ContactsContract.CommonDataKinds.Organization.InterfaceConsts.Label, "Work");
            ops.Add(builder.Build());
            */

            ContentProviderResult[] res = this.ContentResolver.ApplyBatch(ContactsContract.Authority, ops);
            Toast.MakeText(this, "Ocio aggiunto ai contatti, grazie!", ToastLength.Long).Show();
        }

        private class HybridWebViewClient : WebViewClient {
            public override bool ShouldOverrideUrlLoading(WebView webView, string url) {

                return false;
            }
        }
    }
}

