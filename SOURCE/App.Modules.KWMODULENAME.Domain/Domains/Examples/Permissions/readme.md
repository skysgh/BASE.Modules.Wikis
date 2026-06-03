# About #
This folder demonstrates how a logical module can bring its own permission definitions to the platform.

## Current direction ##

The preferred authoring direction is now:

- discover a container type that implements `IPermissionsConfigurationObject`
- define permission keys as constants inside that container
- decorate those constants with `PermissionDefinitionAttribute`
- project those discovered constants into runtime permission configuration objects

This keeps the key as the real logical identity while still giving the framework an explicit discovery boundary to scan.

## Why this is self-registering ##

At startup and during seeding, recognised assemblies are scanned for permission-definition containers.
The attributed constants inside those containers are projected into runtime permission configuration objects, then:

- registered in the runtime permission registry
- validated for duplicate keys and duplicate deterministic IDs
- projected into persisted permission reference data

That means each logical module can bring its own permission definitions without a central hand-maintained list.

## Grouping format ##

Permission grouping metadata is now carried as one semicolon-delimited string named `Grouping`.
The usual shape is:

- `Module;Domain;Group`

Examples:

- `Sys;Settings;Settings`
- `Sys;Settings;Configuration`
- `KWMODULENAME;Examples;Configuration`

Later segments are optional, but the composed string is preferred over scattering grouping data across separate properties.

## Why the runtime model still has more than Key ##

The key is the real logical identity.
The storage `Guid` is derived deterministically from the key.

The projected runtime model still carries richer metadata because the current pipeline also needs:

- `Key` for enforcement identity
- `Title` for admin and UX rendering
- `Description` for admin and UX rendering
- `Enabled` for default state
- `Grouping` for projection, ordering, and grouping
- optional image metadata for richer presentation surfaces

## i18n note ##

The title and description values shown in the Template example are plain strings for readability.
In the long-term design, those should be i18n keys rather than hard-coded user-facing text.
The Template module is explanatory first; it is not trying to be fully production-ready.

## Template example ##

`ExamplePermissionsConfigurationObject` demonstrates:

- a discoverable permission-definition container
- attributed permission constants
- `Grouping` as one composed value
- a permission cluster owned by the logical module itself

This is intentionally only a mini-example.
The current runtime bridge still projects these declarations into the older singular permission runtime contract while migration continues.
