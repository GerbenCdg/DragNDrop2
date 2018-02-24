using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace DragNDrop2.Droid.BlockViews.Container
{
    public abstract class Container : LinearLayout
    {
        protected Container(Context context) : base(context)
        {
            base.Orientation = Orientation.Vertical;
            //LayoutParameters = new ViewGroup.LayoutParams(DefaultWidth, DefaultHeight);
            base.SetBackgroundColor(Color.ParseColor("#FFFFFF"));

            var param = new LinearLayout.LayoutParams(new ViewGroup.MarginLayoutParams(DefaultWidth, DefaultHeight));
            int margin = DpToPx(8);
            param.SetMargins(margin, margin, margin, margin);
            base.LayoutParameters = param;
        }

        public override void AddView(View child)
        {
            SetMargin(child);
            base.AddView(child);
            IncreaseViewSize(child);
        }

        public override void AddView(View child, int index)
        {
            SetMargin(child);
            base.AddView(child, index);
            IncreaseViewSize(child);
        }

        public override void RemoveView(View view)
        {
            base.RemoveView(view);
            DecreaseViewSize(view);
        }


        private void SetMargin(View child)
        {
            int margin = DpToPx(8);
            var param = new ViewGroup.MarginLayoutParams(child.LayoutParameters);
            param.SetMargins(margin, margin, margin, margin);
            child.LayoutParameters = param;
        }

        private void IncreaseViewSize(View child)
        {
            var param = LayoutParameters;
            param.Height = ViewGroup.LayoutParams.WrapContent;
            param.Width= ViewGroup.LayoutParams.WrapContent;

            LayoutParameters = param;

            Invalidate();
            RequestLayout();
        }

        private void DecreaseViewSize(View removedChild)
        {
            if (ChildCount < 1)
            {
                var param = LayoutParameters;
                param.Height = DefaultHeight;
                param.Width = DefaultWidth;

                LayoutParameters = param;

                Invalidate();
                RequestLayout();
            }
        }

        private int DefaultWidth => DpToPx(BlockView.DefaultWidthDp);
        private int DefaultHeight => DpToPx(BlockView.DefaultHeightDp);

        protected int DpToPx(float dp)
        {
            return BlockView.DpToPx(dp, Resources);
        }

    }
}