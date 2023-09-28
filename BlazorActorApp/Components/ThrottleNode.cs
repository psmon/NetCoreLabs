using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace BlazorActorApp.Components
{
    public class ThrottleNode : NodeModel
    {
        public ThrottleNode(Point? position = null) : base(position) { }

        public int ProcessCouuntPerSec { get; set; }

        public double MessageCount { get; set; }

    }
}
