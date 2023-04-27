using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NorskaLib.UI.Widgets
{
    public sealed class WidgetProvider
    {
        private static WidgetProvider instance;
        public static WidgetProvider Instance 
        {
            get => instance ??= new WidgetProvider();
        }

        private List<Widget> widgets = new();
        public IEnumerable<Widget> AllWidgets
            => widgets;

        private List<Widget> activeWidgets = new();
        public IEnumerable<Widget> ActiveWidgets
            => activeWidgets;

        public void RegisterInstance(Widget widget)
        {
            if (!widgets.Contains(widget))
                widgets.Add(widget);
        }

        public void OnWidgetEnabled(Widget widget)
        {
            activeWidgets.Add(widget);
        }

        public void OnWidgetDisabled(Widget widget)
        {
            activeWidgets.Remove(widget);
        }

        public void UnregisterInstance(Widget widget)
        {
            widgets.Remove(widget);
        }

        public bool TryGetWidget(string id, out Widget widget)
        {
            widget = widgets.FirstOrDefault(w => w.Id == id);
            return widget != null;
        }

        public bool TryGetActiveWidget(string id, out Widget widget)
        {
            widget = activeWidgets.FirstOrDefault(w => w.Id == id);
            return widget != null;
        }
    }
}