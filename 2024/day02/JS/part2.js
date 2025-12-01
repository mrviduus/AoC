// https://adventofcode.com/2024/day/2
const fs = require('fs');
const path = require('path');

const INPUT_FILE = path.join(__dirname, '../input.txt');

function readInputFile(filePath) {
  try {
    return fs.readFileSync(filePath, 'utf-8').split('\n');
  } catch (e) {
    console.error(`Error reading file from path ${filePath}:`, e);
    process.exit(1);
  }
}

function parseLines(lines) {
  let reports = [];
  lines.forEach(line => {
    const numbers = line.trim().split(/\s+/);
    const report = numbers.map(Number);
    reports.push(report);
  });
  return reports;
}

function isReportSafe(report) {
  for (let i = 0; i < report.length - 1; i++) {
    if (report[i] >= report[i + 1] || report[i + 1] - report[i] > 3) {
      return false;
    }
  }
  return true;
}

function canBeMadeSafe(report) {
  for (let i = 0; i < report.length; i++) {
    const newReport = report.slice(0, i).concat(report.slice(i + 1));
    if (isReportSafe(newReport)) {
      return true;
    }
  }
  return false;
}

function isReportSafeOrCanBeMadeSafe(report) {
  return isReportSafe(report) || isReportSafe(report.slice().reverse()) || canBeMadeSafe(report) || canBeMadeSafe(report.slice().reverse());
}

function countSafeReports(reports) {
  let safeReports = 0;
  for (let i = 0; i < reports.length; i++) {
    if (isReportSafeOrCanBeMadeSafe(reports[i])) {
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