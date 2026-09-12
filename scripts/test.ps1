# Script de conveniencia para ejecutar pruebas unitarias localmente
param (
    [string]$Filter = "",
    [switch]$Detailed = $false
)

$verbosity = if ($Detailed) { "detailed" } else { "normal" }
$testProj = "MesaFacil.API.UnitTests/MesaFacil.API.UnitTests.csproj"

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host " Ejecutando Suite de Pruebas en MesaFacil.API" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$cmdArgs = @("test", $testProj, "--logger", "console;verbosity=$verbosity")

if ($Filter) {
    Write-Host "Filtro activo: $Filter" -ForegroundColor Yellow
    $cmdArgs += "--filter"
    $cmdArgs += $Filter
}

& dotnet $cmdArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host "[OK] TODAS LAS PRUEBAS PASARON EXITOSAMENTE." -ForegroundColor Green
} else {
    Write-Host "[ERROR] HUBO PRUEBAS FALLIDAS. Codigo de salida: $LASTEXITCODE" -ForegroundColor Red
}

exit $LASTEXITCODE
