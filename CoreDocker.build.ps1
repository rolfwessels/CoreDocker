Framework "4.0"

#
# properties
#

properties {
    $buildConfiguration = 'debug'
    $buildDirectory = 'build'
    $buildReportsDirectory =  Join-Path $buildDirectory 'reports'
    $buildPackageDirectory =  Join-Path $buildDirectory 'packages'
    $buildDistDirectory =  Join-Path $buildDirectory 'dist'
    $buildPublishProjects =  'CoreDocker.Api','CoreDocker.Console'
    $versions = 'netcoreapp2.0','net46'
    $buildContants = ''

    $srcDirectory = 'src'
    $testDirectory = 'test'
    $srcWebFolder = Join-Path $srcDirectory 'CoreDocker.Website'

    $codeCoverRequired = 70

    $versionMajor = 0
    $versionMinor = 0
    $versionBuild = 2
    $versionRevision = 0

    $vsVersion = "12.0"

    $msdeploy = 'C:\Program Files\IIS\Microsoft Web Deploy V3\msdeploy.exe';
    $deployServiceDest = "computerName='xxxx',userName='xxx',password='xxxx',includeAcls='False',tempAgent='false',dirPath='d:\server\temp'"
    $deployApiDest = 'auto,includeAcls="False",tempAgent="false"'
}

#
# task
#

task default -depends build  -Description "By default it just builds"
task clean -depends clean.build,clean.binobj -Description "Removes build folder"
task build -depends clean.binobj,version,build.restore,build.publish,build.copy -Description "Cleans bin/object and builds the project placing binaries in build directory"
task test -depends clean.binobj,build.restore,test.run  -Description "Builds and runs part cover tests"
task full -depends test,build,deploy.zip -Description "Versions builds and creates distributions"
task package -depends version,build,deploy.package -Description "Creates packages that could be user for deployments"
task deploy -depends version,build,deploy.api,deploy.service -Description "Deploy the files to webserver using msdeploy"
task appveyor -depends build,deploy.zip -Description "Runs tests and deploys zip"

#
# task depends
#

task clean.build {
    remove-item -force -recurse $buildDirectory -ErrorAction SilentlyContinue
}

task clean.binobj {
    remove-item -force -recurse $buildReportsDirectory -ErrorAction SilentlyContinue
    remove-item -force -recurse (buildConfigDirectory) -ErrorAction SilentlyContinue
    $binFolders = Get-ChildItem ($srcDirectory + '\*\*') | where { $_.name -eq 'bin' -or $_.name -eq 'obj'} | Foreach-Object {$_.fullname}
    if ($binFolders -ne $null)
    {
        remove-item $binFolders -force -recurse -ErrorAction SilentlyContinue
    }
    $binFolders = Get-ChildItem ($testDirectory + '\*\*') | where { $_.name -eq 'bin' -or $_.name -eq 'obj'} | Foreach-Object {$_.fullname}
    if ($binFolders -ne $null)
    {
        remove-item $binFolders -force -recurse -ErrorAction SilentlyContinue
    }
}

task build.restore {
    'restore '+$buildConfiguration
    dotnet restore -v quiet
    if (!$?) {
        throw 'Failed to restore'
    }
}
task build.build {
    'build '+$buildConfiguration
    dotnet build -v quiet
    if (!$?) {
        throw 'Failed to restore'
    }
}

task version {
    $projectFolders = Get-ChildItem $srcDirectory '*' -Directory
    foreach ($projectFolder in $projectFolders) {
        $projectFile = Get-ChildItem $projectFolder.FullName '*.csproj' -File
        [xml]$Xml = Get-Content $projectFile.FullName
        $result = $Xml.Project.PropertyGroup.Version
        if (![string]::IsNullOrEmpty($result)) {
            $version = (fullversionrev) 
            write-host "Set version $version in  $projectFile" -foreground "magenta"
            $Xml.Project.PropertyGroup.Version = $version
            $Xml.Save( $projectFile.FullName)
        }
    }
}


task build.publish {
    
    foreach ($buildPublishProject in $buildPublishProjects) {
        $toFolder = (Join-Path ( Join-Path (resolve-path .)(buildConfigDirectory)) $buildPublishProject)
        $project =  Join-Path $srcDirectory $buildPublishProject
        # --version-suffix $TRAVIS_BUILD_NUMBER
        Push-Location $project
        
        if ($buildConfiguration -ne 'release') {
            write-host "Publish $project with suffix $buildConfiguration" -foreground "magenta"
            dotnet publish -c $buildConfiguration --version-suffix $buildConfiguration  -v quiet
        }
        else {
            write-host "Publish $project  $buildConfiguration" -foreground "magenta"
            dotnet publish -c $buildConfiguration   -v quiet
        }
        #msbuild   /v:q
        if (!$?) {
            throw "Failed to publish $project"
        }
        
        Pop-Location
    }
    
}

task build.copy {
    foreach ($buildPublishProject in $buildPublishProjects) {
        foreach ($version in $versions ) {
            $fromFolder =  Join-Path $srcDirectory (Join-Path $buildPublishProject (srcBinFolder) )
            $publishFolders = Get-ChildItem -Recurse -Directory $fromFolder | where { $_.name -like '*publish*' -and  $_.fullname -like "*$version*"} | Foreach-Object {$_.fullname}
            foreach ($publishFolder in $publishFolders ) {
                "Copy the $buildPublishProject "+$version
                $toFolder =  Join-Path (Join-Path (buildConfigDirectory) $version) $buildPublishProject
                copy-files $publishFolder $toFolder
            }
        }
    }
}
    


task build.website -depend gulp.build {
    $fromFolder =  Join-Path (srcWebFolder) 'dist'
    $toFolder =  (Join-Path (buildConfigDirectory) 'CoreDocker.Api/static')
    copy-files $fromFolder $toFolder
}

task gulp.build {
    'Yarn install'
    pushd (srcWebFolder)
    yarn install --silent
    'Bower install'
    bower install --silent
    gulp build
    popd
}

task gulp.watch {
    'Guld watch'
    pushd (srcWebFolder)
    gulp serve
    popd
}


task build.nugetPackages -depend build {
    $packagesFolder =  $buildDistDirectory
    mkdir $packagesFolder -ErrorAction SilentlyContinue
    ./src/.nuget/NuGet.exe pack src\CoreDocker.Sdk\CoreDocker.Sdk.csproj -Prop Configuration=$buildConfiguration  -includereferencedprojects
    Move-Item -force *.nupkg $packagesFolder
 }

task publish.nuget {
    $packagesFolder =  $buildDistDirectory
    mkdir $packagesFolder -ErrorAction SilentlyContinue
   ./src/.nuget/NuGet.exe push  ( Join-Path $packagesFolder ('CoreDocker.Sdk.'+(fullversionrev)+'.nupkg'))
}

task nuget.restore {
    ./src/.nuget/NuGet.exe install src\.nuget\packages.config -OutputDirectory lib
}

task clean.database {
   $mongoDbLocations = 'D:\Var\mongodb\bin\mongo.exe','C:\mongodb\bin\mongo.exe','C:\bin\MongoDB\bin\mongo.exe', "D:\Software\MongoDb\bin\mongo.exe",'C:\Program Files\MongoDB\Server\3.2\bin\mongo.exe'
   $mongo = $mongoDbLocations | Where-Object {Test-Path $_} | Select-Object -first 1
   $database = 'CoreDocker_Develop'
   'Use '+ $mongo + ' to drop the database '+$database
   exec { &($mongo) $database  --eval 'db.dropDatabase()' }
}

task test.run -depends build.restore,build.build   -precondition { return $buildConfiguration -eq 'debug' } {
    mkdir $buildReportsDirectory -ErrorAction SilentlyContinue
    $tests = (Get-ChildItem test | % { Join-Path $_.FullName -ChildPath ("bin/Debug/netcoreapp2.0/$($_.Name).dll") }) 
    if ($env:APPVEYOR_JOB_ID) {
        $tests = $tests | Where-Object { $_ -notlike  '*Sdk.Tests*'}
        write-host "Skip sdk tests. (requires db)" -foreground "magenta"
    }
    dotnet vstest /logger:trx $tests 
    Remove-Item $buildReportsDirectory\result.trx -ErrorAction SilentlyContinue
    Move-Item testresults\*.trx $buildReportsDirectory\result.trx -ErrorAction SilentlyContinue
    Remove-Item .\testresults -Force -Recurse -ErrorAction SilentlyContinue
    if (!$?) {
        throw "Failed to test."
    }
}

task deploy.zip {
    mkdir $buildDistDirectory -ErrorAction SilentlyContinue
    $packVersions = Get-ChildItem (buildConfigDirectory) -Directory
    foreach ($packVersion in $packVersions) {
        $appNames = Get-ChildItem $packVersion.FullName -Directory
        foreach ($appName in $appNames) {
            $version = fullversion
            $zipname = Join-Path $buildDistDirectory ($appName.name  + '.v.'+ $version+'.'+$buildConfiguration+'-' + $packVersion +'.zip' )
            write-host ('Create '+$zipname)
            ZipFiles $zipname $appName.fullname
        }
    }
}

task deploy.package {
    $version = fullversion
    $mkdirResult = mkdir $buildPackageDirectory  -ErrorAction SilentlyContinue
    $toFolder = Join-Path ( resolve-path $buildPackageDirectory ) "$buildConfiguration.CoreDocker.Api.v.$version.zip"
    $configuration = $buildConfiguration+';Platform=AnyCPU;AutoParameterizationWebConfigConnectionStrings=false;PackageLocation=' + $toFolder + ';EnableNuGetPackageRestore=true'
    $project = Join-Path $srcDirectory 'CoreDocker.Api\CoreDocker.Api.csproj'
    msbuild /v:q  /t:restorepackages  /T:Package  /p:VisualStudioVersion=$vsVersion /p:Configuration=$configuration  /p:PackageTempRootDir=c:\temp  $project
    if (!$?) {
        throw 'Failed to deploy'
    }
}


task deploy.api -depends deploy.package {
    $deployWebsiteName = 'Default Web Site/CoreDocker.Api.'+$buildConfiguration
    $version = fullversion
    $toFolder = Join-Path ( resolve-path $buildPackageDirectory ) "$buildConfiguration.CoreDocker.Api.v.$version.zip"
    $skip = 'skipAction=Delete,objectName=filePath,absolutePath=Logs'
    $setParam = 'ApplicationPath='+$deployWebsiteName+''
    &($msdeploy) -source:package=$toFolder -dest:$deployApiDest -verb:sync -disableLink:AppPoolExtension -disableLink:ContentExtension -disableLink:CertificateExtension -skip:$skip
    if (!$?) {
        throw 'Failed to deploy'
    }
}

task deploy.service {
    $source = 'dirPath='+( resolve-path (Join-Path (buildConfigDirectory) 'CoreDocker.Console'))
    &($msdeploy) -verb:sync -allowUntrusted -source:$source -dest:$deployServiceDest
    # &($msdeploy) -verb:sync -preSync:runCommand='D:\Dir\on\remote\server\stop-service.cmd',waitInterval=30000 -source:dirPath='C:\dir\of\files\to\be\copied\on\build\server ' -dest:computerName='xx.xx.xx.xx',userName='xx.xx.xx.xx',password='xxxxxxxxxxxxxxx',includeAcls='False',tempAgent='false',dirPath='D:\Dir\on\remote\server\'  -allowUntrusted -postSync:runCommand='D:\Dir\on\remote\server\start-service.cmd',waitInterval=30000
    if (!$?) {
        throw 'Failed to deploy'
    }
}

task ? -Description "Helper to display task info" {
	WriteDocumentation
}

#
# functions
#

function fullversion() {
    $version = $versionBuild
    if ($env:BUILD_NUMBER) {
        $version = $env:BUILD_NUMBER
    }
    if ($env:APPVEYOR_BUILD_NUMBER) {
        $APPVEYOR_BUILD_VERSION 
        $version = $env:APPVEYOR_BUILD_NUMBER
    }
    return "$versionMajor.$versionMajor.$version"
}

function fullversionrev() {
    return  (fullversion) + ".$versionRevision"
}

function srcWebFolder() {
    $possibleWebLocations = ($srcWebFolder + "2"),$srcWebFolder
    $webLocation = $possibleWebLocations | Where-Object {Test-Path $_} | Select-Object -first 1
    write-host 'Found web folder:' $webLocation -foreground "magenta"
    return $webLocation;
}


function srcBinFolder() {
    return  Join-Path bin $buildConfiguration
}

function buildConfigDirectory() {
    Join-Path $buildDirectory $buildConfiguration
}

function global:copy-files($source,$destination,$include=@(),$exclude=@()){
    $sourceFullName = resolve-path $source
    $relativePath = Get-Item $source | Resolve-Path -Relative
    $mkdirResult = mkdir $destination -ErrorAction SilentlyContinue
    $files = Get-ChildItem $source -include $include -Recurse -Exclude $exclude
     foreach ($file in $files) {
       $relativePathOfFile = Get-Item $file.FullName | Resolve-Path -Relative
       $tofile = Join-Path $destination $relativePathOfFile.Substring($relativePath.length)
       Copy-Item -Force $relativePathOfFile $tofile
     }
}

function ZipFiles( $zipfilename, $sourcedir )
{
   del $zipfilename -ErrorAction SilentlyContinue
   Add-Type -Assembly System.IO.Compression.FileSystem
   $compressionLevel = [System.IO.Compression.CompressionLevel]::Optimal
   [System.IO.Compression.ZipFile]::CreateFromDirectory($sourcedir,
        $zipfilename, $compressionLevel, $false)
}

function WriteDocumentation() {
    $currentContext = $psake.context.Peek()

    if ($currentContext.tasks.default) {
        $defaultTaskDependencies = $currentContext.tasks.default.DependsOn
    } else {
        $defaultTaskDependencies = @()
    }

    $docs = $currentContext.tasks.Keys | foreach-object {
        if ($_ -eq "default" -or $_ -eq "?") {
            return
        }

        if ($_ -contains '.') {
            return
        }

        $task = $currentContext.tasks.$_
        new-object PSObject -property @{
            Name = $task.Name;
            Description = $task.Description;
        }
    }

    $docs | where {-not [string]::IsNullOrEmpty($_.Description)} | sort 'Name' | sort 'Description' -Descending | format-table -autoSize -wrap -property Name,Description

    'Examples:'
    '----------'
    ''
    'Clean build directory before executing build:'
    'go clean,build'
    ''
    ''
    'Qa build:'
    'go build -properties @{''buildConfiguration''=''Qa''}'
    ''
    'Staging deploy to sepecified folder:'
    'go deploy -properties @{buildConfiguration=''Staging'';deployServiceDest =''computerName=''''xxxx'''',userName=''''xxx'''',password=''''xxxx'''',includeAcls=''''False'''',tempAgent=''''false'''',dirPath=''''d:\server\temp'''''' }'

}

