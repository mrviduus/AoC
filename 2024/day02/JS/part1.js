// https://adventofcode.com/2024/day/2
const fs = require(`fs`);
const path = require(`path`);

const INPUT_FILE = path.join(__dirname, `../input.txt`);

function readInputFile(filePath){
    try{
        return fs.readFileSync(filePath, `utf-8`).split(`\n`);
    }catch(e){
        console.error(`Error reading file from path ${filePath}:`, e);
        process.exit(1);
    }
}

function parseLines(lines){
    // create structure to store the reports
    let reports = [];
    lines.forEach(line => {
        const numbers = line.trim().split(/\s+/);
        const report = numbers.map(Number);
        reports.push(report);
    });

    return reports;
}

function isReportSave(reports){
    for(let i = 0; i < reports.length; i++){
        if(reports[i]>= reports[i+1] || reports[i+1]- reports[i]>  3){
            return false;
        }
    }
    return true;
}

function countSafeReports(reports){
    let safeReports = 0;
    for(let i =0; i < reports.length; i++){
        if(isReportSave(reports[i])|| isReportSave(reports[i].reverse())){
            safeReports++;
        }
    }
    return safeReports;
}

// Main execution
const inputLines = readInputFile(INPUT_FILE);
const reports = parseLines(inputLines);

const safeReports = countSafeReports(reports);
console.log(safeReports);