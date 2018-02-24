using Android.Content;
using Android.Graphics.Drawables;
using Android.Support.Annotation;
using Android.Support.V4.Content;
using Android.Util;
using Android.Widget;

namespace DragNDrop2.Droid.BlockViews.Simple
{
    public class InstructionBlockView : SimpleBlockView
    {
        public Drawable Drawable { get; }
        public string BlockName { get; }

        public InstructionBlockView(Context context, int drawableRes, string blockName) : base(context)
        {
            Drawable = ContextCompat.GetDrawable(context, drawableRes);
            BlockName = blockName;
            SetDrawable(Drawable);
        }

        public InstructionBlockView(Context context, Drawable drawable, string blockName) : base(context)
        {
            Drawable = drawable;
            BlockName = blockName;
            SetDrawable(Drawable);
        }

        private void SetDrawable(Drawable drawable)
        {
            ImageView imageView = new ImageView(Context);
            imageView.SetImageDrawable(drawable);
            AddView(imageView);

            int padding = DpToPx(12);
            SetPadding(padding, padding, padding, padding);

            Invalidate();
            RequestLayout();
        }

        public override BlockView Clone()
        {
            return new InstructionBlockView(Context, Drawable, BlockName);
        }

        public override Categories[] GetCategories()
        {
            return new []{Categories.Instructions};
        }

        public override string GetBlockName()
        {
            return BlockName;
        }
    }
}