using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Math = System.Math;

namespace IRO.XWebView.Droid.Views
{
    /// <summary>
    /// Copied from https://github.com/TheFinestArtist/FinestWebView-Android/ .
    /// </summary>
    public class ShadowLayout : FrameLayout
    {
        private Color shadowColor;

        private float shadowSize;

        private float cornerRadius;

        private float dx;

        private float dy;

        public ShadowLayout(Context context) :
                base(context)
        {
            SetWillNotDraw(false);
            this.InitAttributes(null);
            SetPadding();
        }

        public ShadowLayout(Context context, IAttributeSet attrs) :
                base(context, attrs)
        {
            SetWillNotDraw(false);
            this.InitAttributes(attrs);
            SetPadding();
        }

        public ShadowLayout(Context context, IAttributeSet attrs, int defStyleAttr) :
                base(context, attrs, defStyleAttr)
        {
            SetWillNotDraw(false);
            this.InitAttributes(attrs);
            SetPadding();
        }

        private void InitAttributes(IAttributeSet attrs)
        {
            TypedArray attr = Context.ObtainStyledAttributes(attrs, Resource.Styleable.ShadowLayout, 0, 0);
            if ((attr == null))
            {
                return;
            }

            try
            {
                //this.cornerRadius = attr.GetDimension(Resource.Styleable.ShadowLayout_slCornerRadius,
                //    Resources.GetDimension(Resource.Dimension.defaultMenuDropShadowCornerRadius));
                //this.shadowSize = attr.GetDimension(Resource.Styleable.ShadowLayout_slShadowSize,
                //    Resources.GetDimension(Resource.Dimension.defaultMenuDropShadowSize));
                //this.dx = attr.GetDimension(Resource.Styleable.ShadowLayout_slDx, 0);
                //this.dy = attr.GetDimension(Resource.Styleable.ShadowLayout_slDy, 0);
                //this.shadowColor = attr.GetColor(Resource.Styleable.ShadowLayout_slShadowColor,
                //    ContextCompat.GetColor(Context, Resource.Color.finestBlack10));
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR -------->\n{ex}");
                throw;
            }
            finally
            {
                attr.Recycle();
            }

        }

        private void SetPadding()
        {
            int xPadding = ((int)((this.shadowSize + Math.Abs(this.dx))));
            int yPadding = ((int)((this.shadowSize + Math.Abs(this.dy))));
            this.SetPadding(xPadding, yPadding, xPadding, yPadding);
        }

        public void SetShadowColor(Color shadowColor)
        {
            this.shadowColor = shadowColor;
            Invalidate();
        }

        public void SetShadowSize(float shadowSize)
        {
            this.shadowSize = shadowSize;
            SetPadding();
        }

        public void SetCornerRadius(float cornerRadius)
        {
            this.cornerRadius = cornerRadius;
            Invalidate();
        }

        public void SetDx(float dx)
        {
            this.dx = dx;
            SetPadding();
        }

        public void SetDy(float dy)
        {
            this.dy = dy;
            SetPadding();
        }


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            //         RoundRectShape rss = new RoundRectShape(new float[]{12f, 12f, 12f,
            //                 12f, 12f, 12f, 12f, 12f}, null, null);
            //         ShapeDrawable sds = new ShapeDrawable(rss);
            //         sds.setShaderFactory(new ShapeDrawable.ShaderFactory() {
            // 
            //             @Override
            //             public Shader resize(int width, int height) {
            //                 LinearGradient lg = new LinearGradient(0, 0, 0, height,
            //                         new int[]{Color.parseColor("#e5e5e5"),
            //                                 Color.parseColor("#e5e5e5"),
            //                                 Color.parseColor("#e5e5e5"),
            //                                 Color.parseColor("#e5e5e5")}, new float[]{0,
            //                         0.50f, 0.50f, 1}, Shader.TileMode.REPEAT);
            //                 return lg;
            //             }
            //         });
            // 
            //         LayerDrawable ld = new LayerDrawable(new Drawable[]{sds, sds});
            //         ld.setLayerInset(0, 5, 5, 0, 0); // inset the shadow so it doesn't start right at the left/top
            //         ld.setLayerInset(1, 0, 0, 5, 5); // inset the top drawable so we can leave a bit of space for the shadow to use
            //SetBackgroundCompat(canvas.Width, canvas.Height);
        }



        private Bitmap CreateShadowBitmap(int shadowWidth, int shadowHeight, float cornerRadius, float shadowSize, float dx, float dy, int shadowColor, Color fillColor)
        {
            Bitmap output = Bitmap.CreateBitmap(shadowWidth, shadowHeight, Bitmap.Config.Alpha8);
            Canvas canvas = new Canvas(output);
            RectF shadowRect = new RectF(this.shadowSize, this.shadowSize, (shadowWidth - this.shadowSize), (shadowHeight - this.shadowSize));
            if ((this.dy > 0))
            {
                shadowRect.Top = (shadowRect.Top + this.dy);
                shadowRect.Bottom = (shadowRect.Bottom - this.dy);
            }
            else if ((this.dy < 0))
            {
                shadowRect.Top = (shadowRect.Top + Math.Abs(this.dy));
                shadowRect.Bottom = (shadowRect.Bottom - Math.Abs(this.dy));
            }

            if ((this.dx > 0))
            {
                shadowRect.Left = (shadowRect.Left + this.dx);
                shadowRect.Right = (shadowRect.Right - this.dx);
            }
            else if ((this.dx < 0))
            {
                shadowRect.Left = (shadowRect.Left + Math.Abs(this.dx));
                shadowRect.Right = (shadowRect.Right - Math.Abs(this.dx));
            }

            Paint shadowPaint = new Paint();
            shadowPaint.AntiAlias=true;
            shadowPaint.Color=fillColor;
            shadowPaint.SetStyle(Paint.Style.Fill);
            shadowPaint.SetShadowLayer(this.shadowSize, this.dx, this.dy, this.shadowColor);
            canvas.DrawRoundRect(shadowRect, this.cornerRadius, this.cornerRadius, shadowPaint);
            return output;
        }


        protected int GetSuggestedMinimumWidth()
        {
            return 0;
        }


        protected int GetSuggestedMinimumHeight()
        {
            return 0;
        }
    }
}