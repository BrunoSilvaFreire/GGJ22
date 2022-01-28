using System;

namespace Shiroi.FX.Effects.Requirements {
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiresFeatureAttribute : Attribute {
        public RequiresFeatureAttribute(params Type[] featureTypes) {
            FeatureTypes = featureTypes;
        }

        public Type[] FeatureTypes { get; set; }

        public int TotalRequiredFeatures => FeatureTypes.Length;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class OptinalFeatureAttribute : Attribute {
        public OptinalFeatureAttribute(params Type[] featureTypes) {
            FeatureTypes = featureTypes;
        }

        public Type[] FeatureTypes { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RequirementsDescription : Attribute {
        public RequirementsDescription(params string[] descriptions) {
            Descriptions = descriptions;
        }

        public string[] Descriptions { get; }
    }
}