using System;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using DragNDrop2.Droid.BlockViews;
using DragNDrop2.Droid.BlockViews.Container;
using DragNDrop2.Droid.BlockViews.Simple;
using Java.Lang;
using Math = Java.Lang.Math;

namespace DragNDrop2.Droid
{
    [Activity(Label = "DragNDrop2", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity, View.IOnDragListener, View.IOnTouchListener
    {
        private const int MaxBlockDepth = 3;
        private ScrollView ScrollView { get; set; }
        private bool SmoothScrolling { get; set; }
        private bool IsScrollingUp { get; set; }

        private Handler Handler { get; set; }
        private RecyclerView Rv { get; set; }
        private LinearLayout Ll { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            EnableFullScreen();

            Rv = FindViewById<RecyclerView>(Resource.Id.main_rv);
            ScrollView = FindViewById<ScrollView>(Resource.Id.main_scrollview);
            Ll = FindViewById<LinearLayout>(Resource.Id.main_ll);

            RecyclerAdapter adapter = new RecyclerAdapter(this, BlockView.Categories.Instructions);
            Rv.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
            Rv.SetAdapter(adapter);

            // TODO selected TAB

            Rv.SetOnDragListener(this);
            Ll.SetOnDragListener(this);

            for (int i = 0; i < 10; i++)
            {
                BlockView textBv = new TextBlockView(this, "BlockView : " + i);
                Ll.AddView(textBv);

                textBv.SetOnTouchListener(this);
                textBv.SetOnDragListener(this);
            }

            Handler = new Handler();

            void SmoothScrollCheck()
            {
                Log("SmoothScrollCheck");
                if (SmoothScrolling)
                {
                    ScrollView.ScrollBy(0, IsScrollingUp ? 15 : -15);
                }

                Handler.PostDelayed(SmoothScrollCheck, 1000 / 60);
            }

            new Runnable(() =>
            {
                Handler.PostDelayed(SmoothScrollCheck, 1000 / 60);
            }).Run();

        }



        private int Count { get; set; }
        private float FirstX { get; set; }
        private float FirstY { get; set; }
        private View LastTouchedView { get; set; }
        private BlockView LastClonedRbv { get; set; } // LastClonedRecycleBlockView
        private bool HasCloned { get; set; }


        public bool OnTouch(View view, MotionEvent motionEvent)
        {
            if (view != LastTouchedView)
            {
                Count = 0;
                LastTouchedView = view;
            }

            if (motionEvent.Action == MotionEventActions.Up)
            {
                Count = 0;
                return true;
            }

            if (Count < 2)
            {
                if (Count == 0)
                {
                    HasCloned = false;
                    FirstX = motionEvent.GetX();
                    FirstY = motionEvent.GetY();
                }

                Count++;
                return true;
            }

            float dX = Math.Abs(motionEvent.GetX() - FirstX);
            float dY = Math.Abs(motionEvent.GetY() - FirstY);

            if (LastTouchedView.Parent is RecyclerView && (dX > dY)
                || LastTouchedView.Parent is LinearLayout && dY > dX)
                return false;

            if (view is RecyclerBlockView recyclerBlockView)
            {
                if (!HasCloned)
                {
                    LastClonedRbv = recyclerBlockView.Clone();
                    LastClonedRbv.SetOnTouchListener(this);
                    LastClonedRbv.SetOnDragListener(this);
                    HasCloned = true;
                }

                view = LastClonedRbv;
            }

            SaveViewPosition(view);

            ClipData clipData = new ClipData("dragged",
                new[] {ClipDescription.MimetypeTextPlain},
                new ClipData.Item("dragged"));

            if (view is RecyclerBlockView)
            {
                view.Visibility = ViewStates.Visible;
            }

            if (view.StartDragAndDrop(clipData, new View.DragShadowBuilder(LastTouchedView), view, 0))
            {
                view.Visibility = ViewStates.Invisible;
            }
            else
            {
                if (LastTouchedView is RecyclerBlockView blockView)
                {
                    blockView.RemoveCloneInstance();
                }
            }

            return true;
        }

        private ViewGroup LastCont { get; set; }
        private int LastPos { get; set; }

        public bool OnDrag(View hoveredView, DragEvent e)
        {
            ViewGroup hoveredContainer = null;
            View dragged = (View) e.LocalState;
            bool hoveredIsRv = hoveredView is RecyclerView;

            Log("HoveredView : " + hoveredView);
            Log("HoveredView.Parent : " + hoveredView.Parent);

            if (hoveredView is LinearLayout layout)
            {
                if (hoveredView.Parent is ScrollView)
                {
                    hoveredContainer = layout;
                }
            }

            if (hoveredView is InstructionContainer || hoveredIsRv) {
            // It can be an InstructionContainer or a ConditionContainer
            hoveredContainer = (ViewGroup) hoveredView;

                // TODO Tabs
                // TODO centrer texte dans RecyclerBlockViews
                // TODO solve problem : Si les InstructionContainer sont acceptés pour le onDrag(true returned dans DragAction.Started)
                // TODO , alors des problemes de scroll apparaissent dans le RecyclerView.
                // TODO scroll automatique aux bords
            
                if (hoveredView is InstructionContainer) {
                if (((BlockView) hoveredView.Parent.Parent).Depth >= MaxBlockDepth
                        && dragged is ContainerBlockView) {
                    Toast.MakeText(this, "Forbidden to nest more than " + MaxBlockDepth + " Blocks in depth.", ToastLength.Long).Show();
                    return false;
                }
            }
        }
            

            switch (e.Action)
            {
                case DragAction.Started:
                    Log("OnDrag Started");
                    // Determines if this View can accept the dragged
                    bool res = hoveredContainer != null;
                    Log("Res : " + res);
                    Log("Dragged : " + dragged);

                    // TODO check this line of code
                    dragged.SetOnDragListener(null);
                    return res;

                case DragAction.Entered:
                    hoveredView.Background.SetColorFilter(Color.ParseColor("#FFE082"), PorterDuff.Mode.SrcIn);
                    hoveredView.Invalidate();

                    return true;

                case DragAction.Exited:
                    hoveredView.Background.ClearColorFilter();
                    hoveredView.Invalidate();

                    if (!hoveredIsRv)
                    {
                        ((ViewGroup) dragged.Parent).RemoveView(dragged);
                    }

                    return true;

                case DragAction.Location:

                    if (hoveredIsRv)
                    {
                        SmoothScrolling = false;
                        return true;
                    }

                    // TODO check out for coordinates : view.onInterceptTouchEvent
                    if (dragged.Parent?.Parent.Parent is ScrollView)
                    {
                        if (e.GetY() < ScrollView.ScrollY + 100)
                        {
                            SmoothScrolling = true;
                            IsScrollingUp = false;
                        }
                        else if (e.GetY() + 150 > ScrollView.ScrollY + ScrollView.Height)
                        {
                            SmoothScrolling = true;
                            IsScrollingUp = true;
                        }
                        else
                        {
                            SmoothScrolling = false;
                        }
                    }

                    AddDraggedView(dragged, hoveredContainer, e.GetY());

                    return true;

                case DragAction.Drop:

                    Log("OnDrag Drop");

                    hoveredView.Background.ClearColorFilter();
                    hoveredView.Invalidate();

                    if (hoveredIsRv)
                    {
                        return true;
                    }

                    if (hoveredContainer != null)
                    {
                        AddDraggedView(dragged, hoveredContainer, e.GetY());
                    }

                    return true;

                case DragAction.Ended:

                    Log("OnDrag Ended. Result : " + (e.Result ? "SUCCESS" : "FAILURE"));

                    SmoothScrolling = false;

                    hoveredView.Background.ClearColorFilter();
                    hoveredView.Invalidate();

                    if (LastTouchedView is RecyclerBlockView view)
                    {
                        view.RemoveCloneInstance();
                        Count = 0; // ensures we will make a new copy if this view is dragged again from the RecyclerView
                    }

                    if (hoveredIsRv) return true;

                    Log("new Parent : " + dragged.Parent + "\n\n");
                    if (!e.Result)
                    {
                        ResetDraggedView(dragged);
                    }

                    // sov 10988671 TODO test this
                    dragged.Post(delegate { dragged.Visibility = ViewStates.Visible; });

                    return true;
            }

            return false;
        }

        private void SaveViewPosition(View dragged)
        {
            if (LastTouchedView is RecyclerBlockView)
            {
                return; // In this case, this view is being dragged from the RecyclerView
            }

            LastCont = (ViewGroup) dragged.Parent;

            for (int i = 0; i < LastCont.ChildCount; i++)
            {
                if (LastCont.GetChildAt(i).Equals(dragged))
                {
                    LastPos = i;
                    return;
                }
            }

            throw new IllegalStateException("Couldn't find the position of the dragged view");
        }

        private void AddDraggedView(View dragged, ViewGroup container, float draggedY)
        {
            View child;

            for (int i = 0; i < container.ChildCount; i++)
            {
                child = container.GetChildAt(i);

                if (draggedY <= (child.GetY() + child.Height / 2f))
                {
                    SetDraggedViewInContainer(dragged, container, i);
                    return;
                }
            }

            // insert at the end
            SetDraggedViewInContainer(dragged, container, container.ChildCount - 1);
        }

        private void SetDraggedViewInContainer(View dragged, ViewGroup container, int position)
        {
            ((ViewGroup) dragged.Parent)?.RemoveView(dragged);
            container.AddView(dragged, position);
        }

        private void ResetDraggedView(View dragged)
        {
            Log("LastTouchedView : " + LastTouchedView);
            if (LastTouchedView is RecyclerBlockView) {
                return;
            }

            SetDraggedViewInContainer(dragged, LastCont, LastPos);
        }

        private void Log(string msg)
        {
            Android.Util.Log.Warn("MainActivity", msg);
        }

        private void EnableFullScreen()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Kitkat)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility) (SystemUiFlags.Fullscreen |
                                                                             SystemUiFlags.LayoutFullscreen |
                                                                             SystemUiFlags.HideNavigation |
                                                                             SystemUiFlags.Immersive |
                                                                             SystemUiFlags.ImmersiveSticky);
            }
        }


    }
}