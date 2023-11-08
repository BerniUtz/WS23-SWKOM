using System.Drawing;

namespace Mock_Server.Controllers;

public static class RandomColorGenerator
{

    public static Color GetRandomColor()
    {
        var rand = new Random();
        var r = rand.Next(256);
        var g = rand.Next(256);
        var b = rand.Next(256);
        return Color.FromArgb(r, g, b);
    }
}
