using System.Resources;

namespace FxResources.LuzFaltex.Extensions.DependencyInjection
{
    internal static class SR
    {
    }
}

namespace System
{
    internal static partial class SR
    {
        private static ResourceManager? s_resourceManager;
        internal static ResourceManager ResourceManager => s_resourceManager ??= new ResourceManager(typeof(FxResources.LuzFaltex.Extensions.DependencyInjection.SR));

        /// <summary>Unable to activate type '{0}'. The following constructors are ambiguous:</summary>
        internal static string @AmbiguousConstructorException => GetResourceString("AmbiguousConstructorException");
        /// <summary>Unable to resolve service for type '{0}' while attempting to activate '{1}'.</summary>
        internal static string @CannotResolveService => GetResourceString("CannotResolveService");
        /// <summary>A circular dependency was detected for the service of type '{0}'.</summary>
        internal static string @CircularDependencyException => GetResourceString("CircularDependencyException");
        /// <summary>No constructor for type '{0}' can be instantiated using services from the service container and default values.</summary>
        internal static string @UnableToActivateTypeException => GetResourceString("UnableToActivateTypeException");
        /// <summary>Open generic service type '{0}' requires registering an open generic implementation type.</summary>
        internal static string @OpenGenericServiceRequiresOpenGenericImplementation => GetResourceString("OpenGenericServiceRequiresOpenGenericImplementation");
        /// <summary>Arity of open generic service type '{0}' does not equal arity of open generic implementation type '{1}'.</summary>
        internal static string @ArityOfOpenGenericServiceNotEqualArityOfOpenGenericImplementation => GetResourceString("ArityOfOpenGenericServiceNotEqualArityOfOpenGenericImplementation");
        /// <summary>Cannot instantiate implementation type '{0}' for service type '{1}'.</summary>
        internal static string @TypeCannotBeActivated => GetResourceString("TypeCannotBeActivated");
        /// <summary>A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.</summary>
        internal static string @NoConstructorMatch => GetResourceString("NoConstructorMatch");
        /// <summary>Cannot consume {2} service '{0}' from {3} '{1}'.</summary>
        internal static string @ScopedInSingletonException => GetResourceString("ScopedInSingletonException");
        /// <summary>Cannot resolve '{0}' from root provider because it requires {2} service '{1}'.</summary>
        internal static string @ScopedResolvedFromRootException => GetResourceString("ScopedResolvedFromRootException");
        /// <summary>Cannot resolve {1} service '{0}' from root provider.</summary>
        internal static string @DirectScopedResolvedFromRootException => GetResourceString("DirectScopedResolvedFromRootException");
        /// <summary>Constant value of type '{0}' can't be converted to service type '{1}'</summary>
        internal static string @ConstantCantBeConvertedToServiceType => GetResourceString("ConstantCantBeConvertedToServiceType");
        /// <summary>Implementation type '{0}' can't be converted to service type '{1}'</summary>
        internal static string @ImplementationTypeCantBeConvertedToServiceType => GetResourceString("ImplementationTypeCantBeConvertedToServiceType");
        /// <summary>'{0}' type only implements IAsyncDisposable. Use DisposeAsync to dispose the container.</summary>
        internal static string @AsyncDisposableServiceDispose => GetResourceString("AsyncDisposableServiceDispose");
        /// <summary>GetCaptureDisposable call is supported only for main scope</summary>
        internal static string @GetCaptureDisposableNotSupported => GetResourceString("GetCaptureDisposableNotSupported");
        /// <summary>Invalid service descriptor</summary>
        internal static string @InvalidServiceDescriptor => GetResourceString("InvalidServiceDescriptor");
        /// <summary>Requested service descriptor doesn't exist.</summary>
        internal static string @ServiceDescriptorNotExist => GetResourceString("ServiceDescriptorNotExist");
        /// <summary>Call site type {0} is not supported</summary>
        internal static string @CallSiteTypeNotSupported => GetResourceString("CallSiteTypeNotSupported");
        /// <summary>Generic implementation type '{0}' has a DynamicallyAccessedMembers attribute applied to a generic argument type, but the service type '{1}' doesn't have a matching DynamicallyAccessedMembers attribute on its generic argument type.</summary>
        internal static string @TrimmingAnnotationsDoNotMatch => GetResourceString("TrimmingAnnotationsDoNotMatch");
        /// <summary>Generic implementation type '{0}' has a DefaultConstructorConstraint ('new()' constraint), but the generic service type '{1}' doesn't.</summary>
        internal static string @TrimmingAnnotationsDoNotMatch_NewConstraint => GetResourceString("TrimmingAnnotationsDoNotMatch_NewConstraint");
    }
}
