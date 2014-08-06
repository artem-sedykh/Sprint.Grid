namespace Sprint.Grid.Impl
{
    using System;
    using System.Linq;
    using System.Web.Mvc;

    internal static class ModelHelper
    {
        internal static bool IsPropertyAllowed(string propertyName, string[] includeProperties, string[] excludeProperties)
        {
            // We allow a property to be bound if its both in the include list AND not in the exclude list.
            // An empty include list implies all properties are allowed.
            // An empty exclude list implies no properties are disallowed.
            var includeProperty = (includeProperties == null) || (includeProperties.Length == 0) || includeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            var excludeProperty = (excludeProperties != null) && excludeProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
            return includeProperty && !excludeProperty;
        }

        internal static TModel GetModel<TModel>(string prefix, string[] includeProperties, string[] excludeProperties, IValueProvider valueProvider, ControllerContext controllerContext, IModelBinder binder = null) where TModel : class
        {
            var modelType = typeof (TModel);
            var modelState = new ModelStateDictionary();

            if (valueProvider == null)
            {
                throw new ArgumentNullException("valueProvider");
            }

            if (controllerContext == null)
            {
                throw new ArgumentNullException("controllerContext");
            }

            Predicate<string> propertyFilter = propertyName => IsPropertyAllowed(propertyName, includeProperties, excludeProperties);


            binder = binder ?? ModelBinders.Binders.GetBinder(modelType);

            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => null, modelType),
                ModelName = prefix,
                ModelState = modelState,
                PropertyFilter = propertyFilter,
                ValueProvider = valueProvider
            };

            return binder.BindModel(controllerContext, bindingContext) as TModel;
        }
    }
}
