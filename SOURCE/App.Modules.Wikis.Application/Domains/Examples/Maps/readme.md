
Always map explicitly (never rely on a mapping libraries 'magic' to do it for you).
The little effort you put into mapping explicitly will pay off in the long run.
It will make your code more readable, maintainable, and less prone to bugs.
Always take the time to define your mappings clearly and avoid relying on implicit behavior
that may change or be misunderstood by others.

In General you need maps for going both ways (Entities to DTOs going out, DTOs to Entities coming back in).
THis is especially important when you have complex objects or when the structure of your DTOs differs significantly
from your entities.
