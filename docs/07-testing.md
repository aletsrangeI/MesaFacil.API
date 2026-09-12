# 07 Testing

## Estrategia de Pruebas
El repositorio cuenta con el proyecto de pruebas `MesaFacil.API.UnitTests` basado en **xUnit**, **Moq**, **FluentAssertions** y **FluentValidation.TestHelper**.

Conforme a la decisión arquitectónica **ADR-004**, la creación y mantenimiento de pruebas unitarias es **obligatoria** para cualquier código que incorpore o modifique lógica de negocio, validaciones o cálculos en el backend.

---

## Estructura del Proyecto de Pruebas
Las pruebas deben organizarse en carpetas que reflejen la estructura de la solución:

```
MesaFacil.API.UnitTests/
├── UseCases/
│   ├── Cuentas/
│   ├── Pagos/
│   ├── Pedidos/
│   ├── Mesas/
│   └── Inventario/
└── Validators/
    ├── CuentaDTOValidatorTests.cs
    ├── PagoDTOValidatorTests.cs
    └── PedidoDTOValidatorTests.cs
```

---

## Qué Probar (Prioridades)

1. **Casos de Uso (`UseCases/`):**
   - Son el núcleo operativo y financiero de MesaFacil.
   - Mockear `IUnitOfWork`, `IMapper` y servicios externos con `Moq`.
   - Probar flujos exitosos (happy path) y ramas de excepción/error de reglas de negocio.
   - Validar efectos colaterales esenciales (ej. al liquidar cuenta, pedido pasa a cerrado, mesa a sucia y se emiten eventos de auditoría).

2. **Validadores (`Validator/`):**
   - Escribir tests para cada validador de DTO usando `FluentValidation.TestHelper`.
   - Probar valores límite (cero, negativos, campos obligatorios vacíos, longitudes máximas).
   - Validar reglas cruzadas y consistencia aritmética (ej. `Total == Subtotal - Descuento + Impuestos`).

3. **Cálculos de Dominio e Inventario:**
   - Métodos de costeo de recetas, conversiones de unidades de medida (G, KG, ML, L) y aplicación de porcentajes de merma.

---

## Patrones y Convenciones Obligatorias

### 1. Patrón AAA (Arrange, Act, Assert)
Todo método de prueba debe estructurarse claramente en tres bloques:
```csharp
[Fact]
public async Task GetAsync_CuandoMesaExiste_RetornaExitoYDto()
{
    // Arrange
    int mesaId = 1;
    _unitOfWorkMock.Setup(uow => uow.Mesas.GetAsync(mesaId)).ReturnsAsync(new Mesa { Id = mesaId });

    // Act
    var result = await _sut.GetAsync(mesaId);

    // Assert
    result.Should().NotBeNull();
    result.isSuccess.Should().BeTrue();
}
```

### 2. Convención de Nomenclatura
Formato: `MetodoAProbar_CondicionOEscenario_ResultadoEsperado`
Ejemplos:
- `GenerarCuentaAsync_ConPagosPrevios_CalculaTotalPagadoYSaldoRestante`
- `RegistrarPagoAsync_PagoCompleto_LiquidaCuentaYCierraPedidoYMarcaMesaSucia`
- `Validate_CuandoMontoEsMenorOIgualACero_DebeTenerError`

### 3. Aserciones
Utilizar **FluentAssertions** para lograr aserciones expresivas:
```csharp
result.isSuccess.Should().BeTrue();
result.Data.SaldoRestante.Should().Be(50.0m);
```

---

## Comandos de Ejecución

### Ejecutar todas las pruebas unitarias
```bash
dotnet test MesaFacil.API/MesaFacil.API.UnitTests/MesaFacil.API.UnitTests.csproj
```

### Ejecutar filtrando por módulo o clase
```bash
dotnet test --filter "FullyQualifiedName~Cuentas"
dotnet test --filter "FullyQualifiedName~Validators"
```

### Script de conveniencia local (Windows PowerShell)
```powershell
.\scripts\test.ps1
```

---

## Quality Gate en CI/CD
El pipeline en Jenkins ejecuta `dotnet test` en modo estricto. Si alguna prueba falla, la compilación se detiene de forma inmediata impidiendo el despliegue automático a producción.