param(
	[Parameter(Mandatory)]
	[string]$version,
	[Parameter(Mandatory)]
	[string]$fromMigration,
	[Parameter(Mandatory)]
	[string]$toMigration
)

$scriptDir = echo $MyInvocation.MyCommand.Path | Split-Path;
$scriptDir | Push-Location;

dotnet ef migrations script -o ".\SQL\${version}_up.sql" -i `
	-p "..\R.Systems.Auth.Infrastructure.Db.csproj" `
	$fromMigration `
	$toMigration;
dotnet ef migrations script -o ".\SQL\${version}_down.sql" -i `
	-p "..\R.Systems.Auth.Infrastructure.Db.csproj" `
	$toMigration `
	$fromMigration;

Pop-Location;