# Setup

1. Create a `configuration.local.json` in this folder, replacing `(local)\\(instance)` with the host and instance name of your SQL Server

		{
		  "DefaultConnection": "Server=(local)\\(instance);Database=sw_develop;Trusted_Connection=True;"
		}
