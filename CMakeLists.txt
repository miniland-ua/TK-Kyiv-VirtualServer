cmake_minimum_required(VERSION 3.20)

project(TK-KYIV-VIRTUALSERVER NONE)

set(CSPROJ "${CMAKE_SOURCE_DIR}/Z21_TC2026.csproj")

add_custom_target(build_app
    COMMAND dotnet build "${CSPROJ}" --configuration $<CONFIG>
    WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
    VERBATIM
)

add_custom_target(run ALL
    COMMAND dotnet run
        --project "${CSPROJ}"
        --configuration $<CONFIG>
        --no-build
    DEPENDS build_app
    WORKING_DIRECTORY "${CMAKE_SOURCE_DIR}"
    USES_TERMINAL
    VERBATIM
)