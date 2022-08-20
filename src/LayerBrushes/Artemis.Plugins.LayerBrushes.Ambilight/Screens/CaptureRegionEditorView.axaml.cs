using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using SkiaSharp;

namespace Artemis.Plugins.LayerBrushes.Ambilight.Screens;

public class CaptureRegionEditorView : ReactiveUserControl<CaptureRegionEditorViewModel>
{
    private readonly Image _displayPreviewImage;
    private readonly Rectangle _regionRectangle;
    private Point _moveOffset;
    private bool _moving;
    private Point _resizeOffset;
    private bool _resizing;

    public CaptureRegionEditorView()
    {
        InitializeComponent();

        _displayPreviewImage = this.Get<Image>("DisplayPreviewImage");
        _regionRectangle = this.Get<Rectangle>("RegionRectangle");

        _displayPreviewImage.LayoutUpdated += DisplayPreviewImageOnLayoutUpdated;
        this.WhenActivated(d =>
        {
            ViewModel.WhenAnyValue(vm => vm.CaptureRegion).Subscribe(UpdateDisplay).DisposeWith(d);
            ViewModel!.PreviewImage = _displayPreviewImage;
        });
    }

    private void DisplayPreviewImageOnLayoutUpdated(object? sender, EventArgs e)
    {
        if (ViewModel != null)
            UpdateDisplay(ViewModel.CaptureRegion);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void UpdateDisplay(Rect rect)
    {
        if (ViewModel == null)
            return;

        // Determine the scale of the preview
        double scaleX = _displayPreviewImage.Bounds.Width / ViewModel.Display.Width;
        double scaleY = _displayPreviewImage.Bounds.Height / ViewModel.Display.Height;

        // Scale down the capture region and apply to the rect 
        _regionRectangle.Width = rect.Width * scaleX;
        _regionRectangle.Height = rect.Height * scaleY;
        Canvas.SetLeft(_regionRectangle, rect.X * scaleX);
        Canvas.SetTop(_regionRectangle, rect.Y * scaleY);

        _displayPreviewImage.InvalidateVisual();
    }

    private void UpdateRegion(SKRectI rect)
    {
        if (ViewModel == null)
            return;

        ViewModel.CapturePropertiesViewModel.X = rect.Left;
        ViewModel.CapturePropertiesViewModel.Y = rect.Top;
        ViewModel.CapturePropertiesViewModel.Width = rect.Width;
        ViewModel.CapturePropertiesViewModel.Height = rect.Height;
    }

    private Point GetScaledPosition(PointerEventArgs e)
    {
        if (ViewModel == null)
            return e.GetPosition(_displayPreviewImage);

        Point position = e.GetPosition(_displayPreviewImage);
        Point normalizedPosition = new(position.X / _displayPreviewImage.Bounds.Width, position.Y / _displayPreviewImage.Bounds.Height);
        return new Point(normalizedPosition.X * ViewModel.Display.Width, normalizedPosition.Y * ViewModel.Display.Height);
    }

    private int LimitVertically(Point position, bool fromTop)
    {
        int result = (int) Math.Round(position.Y, MidpointRounding.AwayFromZero);
        if (ViewModel == null)
            return result;
        return fromTop
            ? Math.Clamp(result, 0, (int) ViewModel.CaptureRegion.Bottom - 1)
            : Math.Clamp(result, (int) ViewModel.CaptureRegion.Top + 1, ViewModel.Display.Height);
    }

    private int LimitHorizontally(Point position, bool fromLeft)
    {
        int result = (int) Math.Round(position.X, MidpointRounding.AwayFromZero);
        if (ViewModel == null)
            return result;
        return fromLeft
            ? Math.Clamp(result, 0, (int) ViewModel.CaptureRegion.Right - 1)
            : Math.Clamp(result, (int) ViewModel.CaptureRegion.Left + 1, ViewModel.Display.Width);
    }

    #region Resizing

    private void ResizeTopLeft(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = ViewModel.CaptureRegion.TopLeft - position;
            StartResize(sender, e);
        }

        // SKRect allows for easy manipulation
        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Top = LimitVertically(position + _resizeOffset, true);
        rect.Left = LimitHorizontally(position + _resizeOffset, true);
        UpdateRegion(rect);
    }

    private void ResizeTop(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = new Point(0, ViewModel.CaptureRegion.Top - position.Y);
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Top = LimitVertically(position + _resizeOffset, true);
        UpdateRegion(rect);
    }

    private void ResizeTopRight(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = ViewModel.CaptureRegion.TopRight - position;
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Top = LimitVertically(position + _resizeOffset, true);
        rect.Right = LimitHorizontally(position + _resizeOffset, false);
        UpdateRegion(rect);
    }

    private void ResizeRight(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = new Point(ViewModel.CaptureRegion.Right - position.X, 0);
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Right = LimitHorizontally(position + _resizeOffset, false);
        UpdateRegion(rect);
    }

    private void ResizeBottomRight(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = ViewModel.CaptureRegion.BottomRight - position;
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Bottom = LimitVertically(position + _resizeOffset, false);
        rect.Right = LimitHorizontally(position + _resizeOffset, false);
        UpdateRegion(rect);
    }

    private void ResizeBottom(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = new Point(0, ViewModel.CaptureRegion.Bottom - position.Y);
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Bottom = LimitVertically(position + _resizeOffset, false);
        UpdateRegion(rect);
    }

    private void ResizeBottomLeft(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = ViewModel.CaptureRegion.BottomLeft - position;
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Bottom = LimitVertically(position + _resizeOffset, false);
        rect.Left = LimitHorizontally(position + _resizeOffset, true);
        UpdateRegion(rect);
    }

    private void ResizeLeft(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);
        if (!_resizing)
        {
            _resizeOffset = new Point(ViewModel.CaptureRegion.Left - position.X, 0);
            StartResize(sender, e);
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        rect.Left = LimitHorizontally(position + _resizeOffset, true);
        UpdateRegion(rect);
    }

    private void StartResize(object? sender, PointerEventArgs e)
    {
        _resizing = true;
        e.Pointer.Capture((IInputElement?) sender);

        // Turn off validation while resizing, otherwise the NumberBox control messes with the rectangle
        if (ViewModel != null)
            ViewModel.CapturePropertiesViewModel.EnableValidation = false;
    }

    private void FinishResize(object? sender, PointerReleasedEventArgs e)
    {
        e.Pointer.Capture(null);
        _resizing = false;

        ViewModel?.ApplyRegion();
    }

    #endregion

    #region Movement

    private void Move(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null || !e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            return;

        Point position = GetScaledPosition(e);

        if (!_moving)
        {
            e.Pointer.Capture((IInputElement?) sender);
            _moveOffset = ViewModel.CaptureRegion.TopLeft - position;
            _moving = true;
        }

        SKRectI rect = ViewModel.GetCaptureRegionRect();
        SKPointI location = new((int) (position + _moveOffset).X, (int) (position + _moveOffset).Y);
        location.X = Math.Clamp(location.X, 0, ViewModel.Display.Width - rect.Width);
        location.Y = Math.Clamp(location.Y, 0, ViewModel.Display.Height - rect.Height);
        rect.Location = location;
        UpdateRegion(rect);
    }

    private void FinishMove(object? sender, PointerReleasedEventArgs e)
    {
        e.Pointer.Capture(null);
        _moving = false;

        ViewModel?.ApplyRegion();
    }

    #endregion
}