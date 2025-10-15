using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace Teendő_Lista
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | Android.Views.SoftInput.StateHidden)]
    [System.Obsolete]
    public class MainActivity : ListActivity
    {
        public List<string> Items { get; set; }

        ArrayAdapter<string> adapter;

        ISharedPreferences prefs = Application.Context.GetSharedPreferences("TODO_DATA", FileCreationMode.Private);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            Items = new List<string>();

            Loadlist();

            adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemMultipleChoice, Items);
            ListAdapter = adapter;

            Button addButton = FindViewById<Button>(Resource.Id.button1);
            addButton.Click += delegate
            {
                EditText itemText = FindViewById<EditText>(Resource.Id.editText1);
                string item = itemText.Text;

                if (item == "" || item == null)
                {
                    return;
                }

                Items.Add(item);

                adapter.Add(item);

                adapter.NotifyDataSetChanged();

                itemText.Text = "";

                UpdatedStoredData();
            };
        }

        [System.Obsolete]
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            base.OnListItemClick(l, v, position, id);

            RunOnUiThread(() =>
            {
                AlertDialog.Builder builder;
                builder = new AlertDialog.Builder(this);
                builder.SetTitle("Elfogadás");
                builder.SetMessage("Befejezted a feladatot?");
                builder.SetCancelable(true);

                builder.SetPositiveButton("OK", delegate
                {
                    var item = Items[position];
                    Items.Remove(item);
                    adapter.Remove(item);

                    l.ClearChoices();
                    l.RequestLayout();

                    UpdatedStoredData();
                });

                builder.SetNegativeButton("Mégse", delegate
                {
                    return;
                });

                builder.Show();
            });
        }
        public void Loadlist()
        {
            int count = prefs.GetInt("itemCount", 0);

            if (count > 0)
            {
                Toast.MakeText(this, "Mentett Elemek Betöltése...", Android.Widget.ToastLength.Short).Show();

                for (int i = 0; i <= count; i++)
                {
                    string item = prefs.GetString(i.ToString(), null);
                    if (item != null)
                    {
                        Items.Add(item);
                    }
                }
            }
        }

        public void UpdatedStoredData()
        {
            ISharedPreferencesEditor editor = prefs.Edit();
            editor.Clear();
            editor.Commit();

            editor = prefs.Edit();

            editor.PutInt("ItemCount", Items.Count);

            int counter = 0;

            foreach (var item in Items)
            {
                editor.PutString(counter.ToString(), item);
                counter++;
            }

            editor.Apply();
        }
    }
}