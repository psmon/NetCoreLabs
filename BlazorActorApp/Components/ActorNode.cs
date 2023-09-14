using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace BlazorActorApp.Components
{
    public class ActorNode : NodeModel
    {
        public ActorNode(Point? position = null) : base(position) { }

        public double MessageCount { get; set; }

    }
}
