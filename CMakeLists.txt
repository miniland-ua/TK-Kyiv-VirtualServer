cmake_minimum_required(VERSION 3.20)

project(TK-KYIV-VIRTUALSERVER NONE)

set(CSPROJ "${CMAKE_SOURCE_DIR}/Z21_TC2026.csproj")

add_custom_target(build_app
    COMMAND powershell -NoProfile -Command
        "Stop-Process -Name Z21_TC2026 -Force -ErrorAction SilentlyContinue; exit 0"
    COMMAND dotnet build "${CSPROJ}" --configuration $<CONFIG>
    WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
    VERBATIM
)

add_custom_target(run ALL
    COMMAND powershell -NoProfile -Command
        "Start-Process '${CMAKE_SOURCE_DIR}/bin/$<CONFIG>/net10.0-windows/Z21_TC2026.exe' -WorkingDirectory '${CMAKE_SOURCE_DIR}'"
    DEPENDS build_app
    WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
    VERBATIM
)