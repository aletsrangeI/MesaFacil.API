# Guía para Agentes de IA - Backend

## Objetivo
Definir cómo trabaja un agente de IA sobre el Backend.

## Alcance
Operaciones, arquitectura y reglas específicas para agentes asistiendo en MesaFacil.API.

## Cuándo consultar este documento
Siempre que un agente de IA vaya a modificar o leer el Backend.

## Información que debe contener
IMPORTANTE: SIEMPRE debe leer primero:
- `../../docs/WORKFLOW.md`
- `../../docs/ROADMAP.md`
- `../../docs/DOMAIN_MODEL.md`
- `../../docs/DECISIONS.md` (Especial atención a **ADR-004**)

Después consultar la documentación del backend en `docs/`:
- `01-architecture.md` (Clean Architecture)
- `07-testing.md` (Estrategia y estándares de pruebas)
- `08-definition-of-done.md` (Criterios obligatorios de finalización)

## Reglas Obligatorias para Agentes de IA

1. **Obligatoriedad de Pruebas Unitarias:**
   - Toda creación o modificación de clases en `UseCases/`, `Validator/` o entidades con lógica de `Domain/` **DEBE** incluir o actualizar sus respectivas pruebas en `MesaFacil.API.UnitTests`.
   - Prohibido entregar código sin pruebas para flujos de negocio nuevos o modificados.
2. **Verificación Previa a la Entrega:**
   - Antes de dar una tarea por terminada, el agente DEBE ejecutar `dotnet test` sobre el proyecto de pruebas y certificar que la suite completa finalice en verde (0 fallos).
3. **Patrones de Prueba:**
   - Seguir estrictamente el patrón **AAA** (Arrange, Act, Assert).
   - Mockear dependencias externas (`IUnitOfWork`, `IMapper`, loggers) con `Moq`.
   - Usar `FluentAssertions` para aserciones claras y legibles.
   - Usar la convención de nomenclatura: `Metodo_Condicion_ResultadoEsperado`.
4. **Respeto a Clean Architecture:**
   - La lógica de negocio y cálculo financiero vive exclusivamente en `UseCases/` o `Domain/`, NUNCA en controladores de `WebApi`.

