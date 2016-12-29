using System;

using UIKit;
using Foundation;
using CoreGraphics;

namespace CycleReferenceDemo
{
	public partial class ViewController : UIViewController
	{


		protected ViewController (IntPtr handle) : base (handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			TestButton ();
			TestScrollView ();
		}

		#region UIScrollView 有 Cycle Reference

		// 如果声明为局部变量，则不会产生循环引用
		private UIScrollView scrollView;
		private void TestScrollView ()
		{
			scrollView = new UIScrollView (new CGRect (100, 100, 100, 100));
			scrollView.BackgroundColor = UIColor.Cyan;
			View.AddSubview (scrollView);

			// 无循环引用
			//scrollView.DraggingStarted += (sender, e) => {

			//};

			// 1. 有循环引用
			//scrollView.DraggingStarted += (sender, e) => {
			//	View.BackgroundColor = UIColor.Green;
			//};

			// 1. 解决方案
			//var wrContext = new WeakReference<UIView> (View);
			//scrollView.DraggingStarted += (sender, e) => {
			//	UIView v;
			//	if (wrContext.TryGetTarget (out v)) {
			//		v.BackgroundColor = UIColor.Green;
			//	}
			//};

			// 2. 有循环引用
			//scrollView.DraggingEnded += ScrollView_DraggingEnded;

			// 2. 解决方案
			//var wrContext2 = new WeakReference<ViewController> (this);
			//scrollView.DraggingEnded += (sender, e) => {
			//	ViewController vc;
			//	if (wrContext2.TryGetTarget (out vc)) {
			//		vc.ScrollView_DraggingEnded (sender, e);
			//	}
			//};

			// 3. 有循环引用
			//scrollView.WeakDelegate = this;

			// 终极解决方案 
			scrollView.Delegate = new MyScrollViewDelegate (this);
		}

		void ScrollView_DraggingEnded (object sender, DraggingEventArgs e)
		{

		}

		void UpdateViewBackgroundColor ()
		{
			View.BackgroundColor = UIColor.Purple;
		}

		class MyScrollViewDelegate:UIScrollViewDelegate
		{
			private readonly WeakReference<ViewController> vc;
			public MyScrollViewDelegate (ViewController viewController)
			{
				this.vc = new WeakReference<ViewController> (viewController);
			}

			public override void Scrolled (UIScrollView scrollView)
			{
				ViewController temp;
				if (vc.TryGetTarget (out temp)) {
					temp.UpdateViewBackgroundColor ();
				}
			}
		}

		#endregion


		#region UIButton 无 Cycle Reference

		private UIButton testButton;
		private void TestButton ()
		{
			testButton = new UIButton (new CGRect (0, 0, 100, 100));
			testButton.BackgroundColor = UIColor.Cyan;
			View.AddSubview (testButton);

			// UIButton 竟然没有产生循环引用
			testButton.TouchUpInside += (sender, e) => {
				View.BackgroundColor = UIColor.Blue;
			};

			testButton.TouchUpInside += TestButton_TouchUpInside; ;
		}

		void TestButton_TouchUpInside (object sender, EventArgs e)
		{

		}

		#endregion
	}
}
