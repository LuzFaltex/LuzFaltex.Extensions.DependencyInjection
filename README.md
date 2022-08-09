# LuzFaltex.Extensions.DependencyInjection
Provides extra capabilities to M.E.DI, such as owned service registrations and named services.

## Goals
This project strives to provide transparent extensions to the existing Microsoft.Extensions.DependencyInjection infrastructure. Consumers of this library will not be required to make any changes to any dependent libraries except to register the new concrete ServiceCollection and ServiceProvider types.

Specifically, the design of this application is as follows:

* Service registrations are named as `Accessibility:OwnerNamespace:ServiceName`. This is known as a Registration Scope.
  * Accessibility modifiers are described below in the accessibility summary table.
  * `OwnerNamespace` is the fully qualified namespace of the library or application which registered the service. This typically matches the name of the NuGet package or the Assembly Name attribute and must be globally unique.
  * The service name is a unique name of the service.
 * Search order for services will be private -> protected -> public. Modifiers to the `GetService()` and `GetRequiredService()` functions can be provided to limit search areas.
 * Each service owner will have the capability to unregister services at runtime. A service owner may register services any time a current registration is not present.
  
 ### Accessibility Summary Table
 
|       **Caller's Location**       | Public | Protected | Private |
|-----------------------------------|:------:|:---------:|:-------:|
| Within the same assembly          |  ✔️️   | ✔️️       | ✔️️     |
| Within a derived assembly         | ✔️️    | ✔️️       | ❌       |
| Within the same execution context | ✔️️    | ❌         | ❌       |

As a practical use case of where this is used, see [Remora.Plugins](https://github.com/Nihlus/Remora.Plugins/issues/4).
* Public service registrations (default) are available to any plugin in the plugin tree. This is how many application-level services are registered, especially when provided for consumption by plugins.
* Protected service registrations are defined by a parent-child relationship. Protected service registrations are accessible by any children.
* Private service registrations are only available to types that live in the same `OwnerNamespace`.

## Integration
This section will cover how to integrate L.E.DI into a parent application or library (such as a plugins library).

## Consumption
This section will cover how to integrate L.E.DI into a child library, such as a particular plugin.
