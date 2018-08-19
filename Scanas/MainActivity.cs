using Java.IO;
using Android.Graphics;
using Environment = Android.OS.Environment;
using Uri = Android.Net.Uri;
using Android.Provider;
using Android.Content;
using System;
using Android;
//using Android.Support.V4.App;
using Android.Hardware;
using System.Threading;
using Android.Views.Animations;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Tesseract.Droid;

public static class Failosas
{
    public static File _file;
    public static File _dir;
    public static Bitmap bitmap;
}



namespace Scanas
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
       private Context mContext;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            mContext = this;

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
            }
            System.Console.WriteLine("App loaded");
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void CreateDirectoryForPictures()
        {
            Failosas._dir = new File(
                Environment.GetExternalStoragePublicDirectory(
                    Environment.DirectoryPictures), "Scanas");
            if (!Failosas._dir.Exists())
            {
                Failosas._dir.Mkdirs();
            }
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            System.Collections.Generic.IList<Android.Content.PM.ResolveInfo> availableActivities =
                PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "OCR Capture", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            // cia darome image capture
            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Camera) == (int)Android.Content.PM.Permission.Granted && Android.Support.V4.App.ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) == (int)Android.Content.PM.Permission.Granted)
            {
                if (Failosas._dir.Exists())
                {
                    Intent intent = new Intent(MediaStore.ActionImageCapture);
                    Failosas._file = new File(Failosas._dir, String.Format("Scanas_{0}.jpg", Guid.NewGuid()));
                    intent.PutExtra(MediaStore.ExtraOutput, Uri.FromFile(Failosas._file));
                    StartActivityForResult(intent, 0);
                } else
                {
                    Android.Widget.Toast.MakeText(this, "Nera sukurtos direktorijos saugoti failams!", Android.Widget.ToastLength.Long).Show();
                }
            }
            else
            {
                 Android.Widget.Toast.MakeText(this, "Reikalinga nustatyti kameros teises!", Android.Widget.ToastLength.Long).Show();
                 Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.Camera, Manifest.Permission.WriteExternalStorage, Manifest.Permission.Flashlight, Manifest.Permission.AccessNetworkState }, 0);
            }
        }

        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            int id = menuItem.ItemId;

            if (id == Resource.Id.nav_camera)
            {

                // Handle the camera action
            }
            else if (id == Resource.Id.nav_gallery)
            {

            }
            else if (id == Resource.Id.nav_slideshow)
            {

            }
            else if (id == Resource.Id.nav_manage)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }


 
        

       protected override async void OnActivityResult(int requestCode, Android.App.Result resultCode, Intent data)
        {

            base.OnActivityResult(requestCode, resultCode, data);
            ProgressDialog progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Kraunama...");
            progress.SetCancelable(false);
            progress.Show();
            try
            {

                System.Console.WriteLine("OnActivityLoad eventas");
                Intent mediaScanIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Uri contentUri = Uri.FromFile(Failosas._file);
                mediaScanIntent.SetData(contentUri);
               // SendBroadcast(mediaScanIntent);

                int height = Android.Content.Res.Resources.System.DisplayMetrics.HeightPixels;
                int width = Android.Content.Res.Resources.System.DisplayMetrics.WidthPixels;    
                Failosas.bitmap = Failosas._file.Path.LoadAndResizeBitmap(width, height);
                if (Failosas.bitmap != null)
                {
                    System.Console.WriteLine("Got bitmap, doing OCR...");

                    TesseractApi api = new TesseractApi(mContext, AssetsDeployment.OncePerInitialization);

                    await api.Init("eng");
                    api.SetPageSegmentationMode(Tesseract.PageSegmentationMode.SparseText);
                    await api.SetImage(Failosas._file.Path);
                        RunOnUiThread(() => {
                                string text = api.Text;
                                System.Console.Error.WriteLine("Gautas text: " + text);
                                Android.Widget.Toast.MakeText(this, "Pagaliau! :D", Android.Widget.ToastLength.Long).Show();
                                progress.Hide();
                                Android.Support.V7.App.AlertDialog.Builder alert = new Android.Support.V7.App.AlertDialog.Builder(this);
                                alert.SetTitle("Gavom teksta");
                                alert.SetMessage(text);
                                alert.SetPositiveButton("nieko gero! :D", (senderAlert, args) =>
                                {
                                    Android.Widget.Toast.MakeText(this, "Gerai kad supratai!", Android.Widget.ToastLength.Short).Show();

                                });
                                Dialog dialog = alert.Create();
                                dialog.Show();
                        });

                } else
                {
                    Android.Widget.Toast.MakeText(this, "Blogas image!", Android.Widget.ToastLength.Short).Show();

                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Klaida darant OCR: " + ex.ToString());
                progress.Hide();
                Android.Widget.Toast.MakeText(this, "Klaida darant OCR!", Android.Widget.ToastLength.Long).Show();

            }
            finally
            {
                GC.Collect();
                progress.Hide();
            }

        }
    }
}

