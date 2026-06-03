Enums are used - but
NEVER used as the Type
of a property of an Entity
that will be persisted.

They are only used to
use to convert the enum
value to a Guid in deterministic
manner.

This is to permit end users to add more
values than just the ones defined
in the enum.

Unless they are Flags, the convention is that Enums *always*
start with Undefined = 0, Unknown = 1, Unspecified = 2.




