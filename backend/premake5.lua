-- Premake5 script to generate a solution for the backend only
-- Usage (run from backend/):
--   premake5 gendsln

newaction({
    trigger = "gendsln",
    description = "Generate TaskManager.sln including application and tests (backend only)",
    execute = function()
        local backendDir = os.getcwd()

        print("[premake] Generating solution in: " .. backendDir)

        local function run(cmd)
            print("[premake] " .. cmd)
            os.execute(cmd)
        end

        -- Create or overwrite solution in backend folder
        run("dotnet new sln -n TaskManager --force")

        -- Add projects relative to backend folder
        run("dotnet sln TaskManager.sln add Application/task-manager-api.csproj")
        run("dotnet sln TaskManager.sln add tests/TaskManager.UnitTests/TaskManager.UnitTests.csproj")
        run("dotnet sln TaskManager.sln add tests/TaskManager.IntegrationTests/TaskManager.IntegrationTests.csproj")

        print("[premake] Solution generated: TaskManager.sln")
    end
})


