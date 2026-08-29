namespace GlobalIntelligence.Utils.Helpers;

public static class AnimationHelper
{
    public static async Task FadeInAsync(View view, uint duration = 250)
    {
        view.Opacity = 0;
        await view.FadeTo(1, duration);
    }

    public static async Task FadeOutAsync(View view, uint duration = 250)
    {
        await view.FadeTo(0, duration);
    }

    public static async Task ScaleAsync(View view, double scale, uint duration = 250)
    {
        await view.ScaleTo(scale, duration);
    }

    public static async Task PulseAsync(View view, uint duration = 500)
    {
        await view.ScaleTo(1.1, duration / 2);
        await view.ScaleTo(1.0, duration / 2);
    }

    public static async Task ShakeAsync(View view, uint duration = 500)
    {
        var steps = 4;
        var stepDuration = duration / (uint)steps;

        for (int i = 0; i < steps; i++)
        {
            await view.TranslateTo(10, 0, stepDuration / 2);
            await view.TranslateTo(-10, 0, stepDuration / 2);
        }

        await view.TranslateTo(0, 0, 0);
    }
}
