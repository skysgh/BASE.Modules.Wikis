# About #
The terms Configuration and Settings are often used interchangeably, but they are better treated as two operating modes of a broader configurable-value model.

All configurable values are canonical keyed definitions with type and metadata.
“Configuration” and “Settings” are two operating modes of the same broader definition model.
Configuration is typically startup-oriented, host-sourced, and runtime-immutable.
Settings are typically runtime-resolved, scope-aware, user-presentable, and potentially mutable.

## Shared shape ##

Both configuration values and settings usually benefit from the same core descriptive elements:

- a canonical hierarchical path or key
- a scalar or structured type
- a default or baseline value
- display metadata such as title and description
- renderability and mutability guidance

That shared shape is why they should not be modeled as completely unrelated concepts.
The more useful distinction is behavioural rather than structural.

## Configuration ##

Configuration usually describes values that are:

- resolved from host configuration sources such as appsettings, environment variables, deployment pipelines, or key vault
- needed at startup or infrastructure boundaries
- normally not editable by ordinary users at runtime
- often immutable once the service is running

Configuration may still be visible in diagnostics or maintainer-facing views.
Visible does not imply mutable.

## Settings ##

Settings usually describe values that are:

- resolved at runtime
- influenced by scope or hierarchy
- potentially editable by privileged users or end users
- expected to carry display metadata suitable for rendering in a UI

Examples include user preferences, workspace preferences, branding values, and feature toggles intended for runtime governance.

## Key architectural consequences ##

### Canonical identity

Configurable values should have a canonical pathed key defined by constants rather than magic strings.
The path supports organisation, discoverability, and stable identity.

### Metadata-first treatment

If a value may ever need to be shown, audited, or rendered, it should carry metadata such as:

- title
- description
- serialised default value
- renderability
- runtime mutability

### Runtime behaviour matters

The important distinction is not merely where a value is stored.
It is also about:

- when it is resolved
- whether it can be changed at runtime
- whether it participates in scope inheritance
- whether it can be presented to users
- whether it can be reset, locked, or overridden

## Grouping is not the same as path ##

Hierarchical paths are useful for canonical identity, but they are not enough by themselves to define good UX grouping.

For example, a path such as:

- `App:Modules:FooApplet:BarSubArea:Key`

is a perfectly valid canonical key, but it does not tell the renderer where the UX should create page headers, sections, or nested groups.
A naive expandable tree based only on path segments is logical, but often poor UX.

That means path and grouping must be treated as related but different concerns:

- the path identifies the configurable value
- grouping metadata decides how values are presented to people

The practical implication is that the framework will likely need explicit grouping metadata in addition to canonical path metadata.
That grouping may later support:

- group title
- subgroup title
- group order
- optional page-level composition

So yes: hierarchical path alone is not sufficient to define human-friendly settings UX.

## Nested service configuration ##

The template also demonstrates that a module-level `IConfigurationObject` can contain nested `IServiceConfiguration` subtrees.

That is a coherent pattern.
It is useful when:

- the module owns the broader configuration section
- one injected service needs a smaller dedicated subtree
- different implementations of that service need different nested configuration objects

In that model:

- the `IConfigurationObject` owns the top-level module section
- a property such as `AnotherExampleServiceImplementation` selects the service implementation
- nested `IServiceConfiguration` objects such as `ServiceA` and `ServiceB` hold implementation-specific values

This keeps configuration coherent while still allowing one service to have a focused injectable configuration model.

## Template example ##

In this template module, `ExampleConfigurationObject` demonstrates:

- module-level configuration through `IConfigurationObject`
- property-level canonical metadata through `ConfigurationSettingSource`
- nested service-specific configuration through `IServiceConfiguration`
- implementation-specific subtrees through `AnotherExampleAServiceConfiguration` and `AnotherExampleBServiceConfiguration`

This is intentionally only a mini-example.
The full framework direction should later add richer discovery, effective-value provenance, permissions, sensitivity, and dedicated grouping metadata for rendering.


