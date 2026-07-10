# 08 Definition of Done

Cada nueva funcionalidad, historia de usuario o corrección de error se considerará "Terminada" únicamente cuando cumpla con los siguientes criterios:

## Checklist de Desarrollo
- [ ] **Clean Architecture:** Se respetó la regla de dependencia (Ninguna capa interior depende de una capa exterior). Lógica de negocio en UseCases/Domain, NO en WebApi.
- [ ] **Convenciones de Código:** El código sigue los estándares definidos en `03-coding-standards.md` (Async/Await, Naming, CancellationToken).
- [ ] **Documentación:** Se actualizaron modelos Mermaid y este set de documentación si hubo cambios arquitectónicos o de modelo de dominio.
- [ ] **Migraciones DB:** Si hubo cambios en el modelo de dominio (`Domain/Entities`), se generó la respectiva migración en `Persistence` y fue probada localmente.

## Checklist de Pruebas (Cuando aplique)
- [ ] **Tests de UseCases:** Existe prueba unitaria para los casos de éxito y de fallo (regla de negocio no cumplida) del UseCase creado o modificado.
- [ ] **Validaciones:** Se probó (vía Postman/Scalar y tests unitarios) que FluentValidation rechaza requests mal formados apropiadamente devolviendo HTTP 400.

## Checklist de Integración
- [ ] **Inyección de Dependencias:** Todos los nuevos repositorios, servicios y casos de uso están registrados apropiadamente (vía Scrutor u explícitamente) y no generan errores de resolución en el arranque del servidor.
- [ ] **Advertencias del Compilador:** La compilación debe ser limpia. No se introdujeron nuevos warnings o están debidamente suprimidos/justificados.
