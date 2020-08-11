﻿using Avalonia.Controls;
using Avalonia.Layout;
using OpenTracker.Models.Settings;
using ReactiveUI;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Reactive;

namespace OpenTracker.ViewModels
{
    /// <summary>
    /// This is the ViewModel class for the top menu control.
    /// </summary>
    public class TopMenuVM : ViewModelBase
    {
        public bool DisplayAllLocations =>
            AppSettings.Instance.Tracker.DisplayAllLocations;
        public bool ShowItemCountsOnMap =>
            AppSettings.Instance.Tracker.ShowItemCountsOnMap;

        public bool DynamicLayoutOrientation =>
            AppSettings.Instance.Layout.LayoutOrientation == null;
        public bool HorizontalLayoutOrientation =>
            AppSettings.Instance.Layout.LayoutOrientation == Orientation.Horizontal;
        public bool VerticalLayoutOrientation =>
            AppSettings.Instance.Layout.LayoutOrientation == Orientation.Vertical;

        public bool DynamicMapOrientation =>
            AppSettings.Instance.Layout.MapOrientation == null;
        public bool HorizontalMapOrientation =>
            AppSettings.Instance.Layout.MapOrientation == Orientation.Horizontal;
        public bool VerticalMapOrientation =>
            AppSettings.Instance.Layout.MapOrientation == Orientation.Vertical;

        public bool TopHorizontalUIPanelPlacement =>
            AppSettings.Instance.Layout.HorizontalUIPanelPlacement == Dock.Top;
        public bool BottomHorizontalUIPanelPlacement =>
            AppSettings.Instance.Layout.HorizontalUIPanelPlacement == Dock.Bottom;

        public bool LeftVerticalUIPanelPlacement =>
            AppSettings.Instance.Layout.VerticalUIPanelPlacement == Dock.Left;
        public bool RightVerticalUIPanelPlacement =>
            AppSettings.Instance.Layout.VerticalUIPanelPlacement == Dock.Right;

        public bool LeftHorizontalItemsPlacement =>
            AppSettings.Instance.Layout.HorizontalItemsPlacement == Dock.Left;
        public bool RightHorizontalItemsPlacement =>
            AppSettings.Instance.Layout.HorizontalItemsPlacement == Dock.Right;

        public bool TopVerticalItemsPlacement =>
            AppSettings.Instance.Layout.VerticalItemsPlacement == Dock.Top;
        public bool BottomVerticalItemsPlacement =>
            AppSettings.Instance.Layout.VerticalItemsPlacement == Dock.Bottom;

        public bool NoneUIScale =>
            AppSettings.Instance.Layout.UIScale == 1.0;
        public bool TwentyFivePercentUIScale =>
            AppSettings.Instance.Layout.UIScale == 1.25;
        public bool FiftyPercentUIScale =>
            AppSettings.Instance.Layout.UIScale == 1.50;
        public bool SeventyFivePercentUIScale =>
            AppSettings.Instance.Layout.UIScale == 1.75;
        public bool OneHundredPercentUIScale =>
            AppSettings.Instance.Layout.UIScale == 2.0;

        public ReactiveCommand<Unit, Unit> OpenResetDialogCommand { get; }
        public ReactiveCommand<Unit, Unit> UndoCommand { get; }
        public ReactiveCommand<Unit, Unit> RedoCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleDisplayAllLocationsCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleShowItemCountsOnMapCommand { get; }
        public ReactiveCommand<string, Unit> SetLayoutOrientationCommand { get; }
        public ReactiveCommand<string, Unit> SetMapOrientationCommand { get; }
        public ReactiveCommand<string, Unit> SetHorizontalUIPanelPlacementCommand { get; }
        public ReactiveCommand<string, Unit> SetVerticalUIPanelPlacementCommand { get; }
        public ReactiveCommand<string, Unit> SetHorizontalItemsPlacementCommand { get; }
        public ReactiveCommand<string, Unit> SetVerticalItemsPlacementCommand { get; }
        public ReactiveCommand<string, Unit> SetUIScaleCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mainWindow">
        /// The ViewModel of the main window.
        /// </param>
        public TopMenuVM(MainWindowVM mainWindow)
        {
            if (mainWindow == null)
            {
                throw new ArgumentNullException(nameof(mainWindow));
            }

            OpenResetDialogCommand = mainWindow.OpenResetDialogCommand;
            UndoCommand = mainWindow.UndoCommand;
            RedoCommand = mainWindow.RedoCommand;
            ToggleDisplayAllLocationsCommand = mainWindow.ToggleDisplayAllLocationsCommand;
            ToggleShowItemCountsOnMapCommand = ReactiveCommand.Create(ToggleShowItemCountsOnMap);
            SetLayoutOrientationCommand = ReactiveCommand.Create<string>(SetLayoutOrientation);
            SetMapOrientationCommand = ReactiveCommand.Create<string>(SetMapOrientation);
            SetHorizontalUIPanelPlacementCommand = ReactiveCommand.Create<string>(SetHorizontalUIPanelPlacement);
            SetVerticalUIPanelPlacementCommand = ReactiveCommand.Create<string>(SetVerticalUIPanelPlacement);
            SetHorizontalItemsPlacementCommand = ReactiveCommand.Create<string>(SetHorizontalItemsPlacement);
            SetVerticalItemsPlacementCommand = ReactiveCommand.Create<string>(SetVerticalItemsPlacement);
            SetUIScaleCommand = ReactiveCommand.Create<string>(SetUIScale);

            AppSettings.Instance.Tracker.PropertyChanged += OnTrackerSettingsChanged;
            AppSettings.Instance.Layout.PropertyChanged += OnLayoutChanged;
        }

        /// <summary>
        /// Subscribes to the PropertyChanged event on the TrackerSettings class.
        /// </summary>
        /// <param name="sender">
        /// The sending object of the event.
        /// </param>
        /// <param name="e">
        /// The arguments of the PropertyChanged event.
        /// </param>
        private void OnTrackerSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TrackerSettings.DisplayAllLocations))
            {
                this.RaisePropertyChanged(nameof(DisplayAllLocations));
            }

            if (e.PropertyName == nameof(TrackerSettings.ShowItemCountsOnMap))
            {
                this.RaisePropertyChanged(nameof(ShowItemCountsOnMap));
            }
        }

        /// <summary>
        /// Subscribes to the PropertyChanged event on the LayoutSettings class.
        /// </summary>
        /// <param name="sender">
        /// The sending object of the event.
        /// </param>
        /// <param name="e">
        /// The arguments of the PropertyChanged event.
        /// </param>
        private void OnLayoutChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(LayoutSettings.LayoutOrientation))
            {
                this.RaisePropertyChanged(nameof(DynamicLayoutOrientation));
                this.RaisePropertyChanged(nameof(HorizontalLayoutOrientation));
                this.RaisePropertyChanged(nameof(VerticalLayoutOrientation));
            }

            if (e.PropertyName == nameof(LayoutSettings.MapOrientation))
            {
                this.RaisePropertyChanged(nameof(DynamicMapOrientation));
                this.RaisePropertyChanged(nameof(HorizontalMapOrientation));
                this.RaisePropertyChanged(nameof(VerticalMapOrientation));
            }

            if (e.PropertyName == nameof(LayoutSettings.HorizontalUIPanelPlacement))
            {
                this.RaisePropertyChanged(nameof(TopHorizontalUIPanelPlacement));
                this.RaisePropertyChanged(nameof(BottomHorizontalUIPanelPlacement));
            }

            if (e.PropertyName == nameof(LayoutSettings.VerticalUIPanelPlacement))
            {
                this.RaisePropertyChanged(nameof(LeftVerticalUIPanelPlacement));
                this.RaisePropertyChanged(nameof(RightVerticalUIPanelPlacement));
            }

            if (e.PropertyName == nameof(LayoutSettings.HorizontalItemsPlacement))
            {
                this.RaisePropertyChanged(nameof(LeftHorizontalItemsPlacement));
                this.RaisePropertyChanged(nameof(RightHorizontalItemsPlacement));
            }

            if (e.PropertyName == nameof(LayoutSettings.VerticalItemsPlacement))
            {
                this.RaisePropertyChanged(nameof(TopVerticalItemsPlacement));
                this.RaisePropertyChanged(nameof(BottomVerticalItemsPlacement));
            }

            if (e.PropertyName == nameof(LayoutSettings.UIScale))
            {
                this.RaisePropertyChanged(nameof(NoneUIScale));
                this.RaisePropertyChanged(nameof(TwentyFivePercentUIScale));
                this.RaisePropertyChanged(nameof(FiftyPercentUIScale));
                this.RaisePropertyChanged(nameof(SeventyFivePercentUIScale));
                this.RaisePropertyChanged(nameof(OneHundredPercentUIScale));
            }
        }

        /// <summary>
        /// Toggles whether to show the item counts on the map.
        /// </summary>
        private void ToggleShowItemCountsOnMap()
        {
            AppSettings.Instance.Tracker.ShowItemCountsOnMap =
                !AppSettings.Instance.Tracker.ShowItemCountsOnMap;
        }

        /// <summary>
        /// Sets the layout orientation to the specified value.
        /// </summary>
        /// <param name="orientationString">
        /// A string representing the new layout orientation value.
        /// </param>
        private void SetLayoutOrientation(string orientationString)
        {
            if (orientationString == "Dynamic")
            {
                AppSettings.Instance.Layout.LayoutOrientation = null;
            }
            else if (Enum.TryParse(orientationString, out Orientation orientation))
            {
                AppSettings.Instance.Layout.LayoutOrientation = orientation;
            }
        }

        /// <summary>
        /// Sets the map orientation to the specified value.
        /// </summary>
        /// <param name="orientationString">
        /// A string representing the new map orientation value.
        /// </param>
        private void SetMapOrientation(string orientationString)
        {
            if (orientationString == "Dynamic")
            {
                AppSettings.Instance.Layout.MapOrientation = null;
            }
            else if (Enum.TryParse(orientationString, out Orientation orientation))
            {
                AppSettings.Instance.Layout.MapOrientation = orientation;
            }
        }

        /// <summary>
        /// Sets the horizontal UI panel orientation to the specified value.
        /// </summary>
        /// <param name="dockString">
        /// A string representing the new horizontal UI panel orientation value.
        /// </param>
        private void SetHorizontalUIPanelPlacement(string dockString)
        {
            if (Enum.TryParse(dockString, out Dock dock))
            {
                AppSettings.Instance.Layout.HorizontalUIPanelPlacement = dock;
            }
        }

        /// <summary>
        /// Sets the vertical UI panel orientation to the specified value.
        /// </summary>
        /// <param name="dockString">
        /// A string representing the new vertical UI panel orientation value.
        /// </param>
        private void SetVerticalUIPanelPlacement(string dockString)
        {
            if (Enum.TryParse(dockString, out Dock dock))
            {
                AppSettings.Instance.Layout.VerticalUIPanelPlacement = dock;
            }
        }

        /// <summary>
        /// Sets the horizontal items placement orientation to the specified value.
        /// </summary>
        /// <param name="dockString">
        /// A string representing the new horizontal items placement orientation value.
        /// </param>
        private void SetHorizontalItemsPlacement(string dockString)
        {
            if (Enum.TryParse(dockString, out Dock dock))
            {
                AppSettings.Instance.Layout.HorizontalItemsPlacement = dock;
            }
        }

        /// <summary>
        /// Sets the vertical items placement orientation to the specified value.
        /// </summary>
        /// <param name="dockString">
        /// A string representing the new vertical items placement orientation value.
        /// </param>
        private void SetVerticalItemsPlacement(string dockString)
        {
            if (Enum.TryParse(dockString, out Dock dock))
            {
                AppSettings.Instance.Layout.VerticalItemsPlacement = dock;
            }
        }

        /// <summary>
        /// Sets the UI scale to the specified value.
        /// </summary>
        /// <param name="uiScaleValue">
        /// A floating point number representing the UI scale value.
        /// </param>
        private static void SetUIScale(string uiScaleValue)
        {
            AppSettings.Instance.Layout.UIScale = double.Parse(uiScaleValue, CultureInfo.InvariantCulture);
        }
    }
}