using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace DragNDrop2.Droid.BlockViews
{
    public abstract class BlockView : CardView
    {
        public const int DefaultWidthDp = 200;
        public const int DefaultHeightDp = 120;

        protected BlockView(Context context) : base(context)
        {
            Initialize();
        }

        protected BlockView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        private void Initialize()
        {
            var param = new LinearLayout.LayoutParams(DefaultMarginLayoutParams);
            int margin = DpToPx(16);
            param.SetMargins(margin, margin, margin, margin);
            LayoutParameters = param;

            base.Radius = 8;
            Invalidate();
            RequestLayout();
        }

        public abstract bool IsContainer();
        public new abstract BlockView Clone();
        public abstract Categories[] GetCategories();
        public abstract String GetBlockName();

        protected int DpToPx(float dp)
        {
            return DpToPx(dp, Resources);
        }

        public static int DpToPx(float dp, Resources res)
        {
            return (int) TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, res.DisplayMetrics);
        }

        public int DefaultWidth => DpToPx(DefaultWidthDp);
        public int DefaultHeight => DpToPx(DefaultHeightDp);

        public ViewGroup.MarginLayoutParams DefaultMarginLayoutParams =>
            new ViewGroup.MarginLayoutParams(DefaultWidth, DefaultHeight);

        public int Depth
        {
            get
            {
                int depth = 0;

                IViewParent iterView = this;
                for (; iterView is BlockView; depth++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (iterView != null)
                            iterView = iterView.Parent;
                        else return depth;
                    }
                }

                return depth;
            }
        }


        public enum Categories
        {
            Instructions,
            All,
            Logic,
            Containers,
            Others
        }
    }
}