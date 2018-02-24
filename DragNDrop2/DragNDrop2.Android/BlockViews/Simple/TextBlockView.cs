using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DragNDrop2.Droid.BlockViews.Simple
{
    public class TextBlockView : SimpleBlockView
    {
        private TextView TextView { get; set; }
        public string Text => TextView.Text;

        public TextBlockView(Context context) : base(context)
        {
            InflateTextView();
        }

        public TextBlockView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            InflateTextView();
        }

        public TextBlockView(Context context, string text) : base(context)
        {
            InflateTextView();
            TextView.Text = text;
        }


        private void InflateTextView()
        {
            TextView = new TextView(Context);
            TextView.SetTextSize(ComplexUnitType.Sp, 16);
            TextView.Gravity = GravityFlags.Center;

            int padding = DpToPx(12);
            SetPadding(padding, padding, padding, padding);

            AddView(TextView);

            Invalidate();
            RequestLayout();
        }

        public override BlockView Clone()
        {
            return new TextBlockView(Context, Text);
        }

        public override Categories[] GetCategories()
        {
            return new []{Categories.Instructions};
        }

        public override string GetBlockName()
        {
            return Text;
        }
    }
}