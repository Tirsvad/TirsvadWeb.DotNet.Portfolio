#!/bin/bash

src="/mnt/e/repos/TirsvadWeb.DotNet.Portfolio/TestResults/CoverageReport/Latest/Html"
dest="dev.tirsvad.dk:/srv/http/dev.tirsvad.dk/www/projects/TirsvadWeb/DotNet.Portfolio/codecoverage/"

rsync -avz --delete --rsync-path="sudo rsync" ${src} ${dest}
