# 07 Testing

## Estrategia de Pruebas
Actualmente el repositorio no muestra proyectos explícitos de pruebas (`Tests`, `UnitTests`), pero para futuras funcionalidades y módulos, se deben seguir las siguientes pautas.

## Qué Probar (Prioridades)
1. **Lógica de Dominio:** Comportamiento dentro de las entidades (si se añade lógica rica en lugar de modelo anémico).
2. **UseCases (Application Logic):** Son el corazón del sistema. Se deben inyectar repositorios *Mockeados* (ej. Moq o NSubstitute) para validar los flujos de orquestación, retornos exitosos y fallos de reglas de negocio.
3. **Validadores (FluentValidation):** Escribir tests para cada validador de DTO verificando qué ocurre con datos inválidos, límites y casos de éxito.

## Patrones a Seguir
- **AAA (Arrange, Act, Assert):** Estructura básica de cualquier prueba.
- **Inyección de Dependencias en Pruebas:** Instanciar las clases concretas (Ej. `CrearPedidoUseCase`) inyectando interfaces falsas (`IUnitOfWork`, `IPedidoRepository`).
- **Librerías recomendadas:** `xUnit` (framework de testing), `Moq` / `NSubstitute` (mocking), `FluentAssertions` (aserciones legibles).

## Naming de Pruebas
- El nombre del test debe describir la intención. 
- Formato sugerido: `MetodoAProbar_Condicion_ResultadoEsperado`
- Ejemplo: `CreatePedidoAsync_WithInvalidMesaId_ReturnsError`
