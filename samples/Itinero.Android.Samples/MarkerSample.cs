using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Itinero.Android.MapMarkers;

namespace Itinero.Android.Samples
{
    class MarkerSample
    {
        public static LinearLayout CreateCustomView(Context context)
        {
            var linearLayout = new LinearLayout(context);
            var relativeLayoutParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.MatchParent);
            relativeLayoutParams.AddRule(LayoutRules.CenterHorizontal);
            var button = new Button(context) { Text = "Show more" };
            button.Click += (sender, args) => Toast.MakeText(context, "more", ToastLength.Long).Show();
            linearLayout.AddView(button);
            var textView = new TextView(context)
            {
                Text = "Cape town",
                Background = new ColorDrawable(Color.Argb(128, 0, 0, 0)),
                LayoutParameters = relativeLayoutParams,
                Gravity = GravityFlags.Center
            };

            linearLayout.AddView(textView);
            linearLayout.Orientation = Orientation.Vertical;
            return linearLayout;
        }
    }
}
