import http from 'k6/http';
import { Trend } from 'k6/metrics';
import { Counter } from 'k6/metrics';

const addDuration = new Trend('add_duration');
const containsDuration = new Trend('contains_duration');
const removeDuration = new Trend('remove_duration');

const addCount = new Counter('add_count');
const containsCount = new Counter('contains_count');
const removeCount = new Counter('remove_count');

const recordsCount = new Trend('records_count');

const kind = `${__ENV.kind}`
const timeout = `${__ENV.timeout}`
const requestsPerSecond = `${__ENV.rps}`
const addRequestsPercent = `${__ENV.add}`
const containsRequestsPercent = `${__ENV.contains}`
const removeRequestsPercent = `${__ENV.remove}`
const durationS = `${__ENV.duration}`

const containsRequestsPerSecond = Math.floor(requestsPerSecond * containsRequestsPercent / 100.0)
const addRequestsPerSecond = Math.floor(requestsPerSecond * addRequestsPercent / 100.0)
const removeRequestsPerSecond = Math.floor(requestsPerSecond * removeRequestsPercent / 100.0)
const maxRecordsCount = requestsPerSecond * durationS * 0.1

const ip = "ip_here"

const params = {
    timeout: `${timeout}s`
  };

const addScenario = {
    executor: 'constant-arrival-rate',
    rate: addRequestsPerSecond,
    timeUnit: '1s',
    duration: `${durationS}s`,
    preAllocatedVUs: 10,
    maxVUs: 3000,
    exec: 'onlyAdd',
    gracefulStop: '20s'
}

const containsScenario = {
    executor: 'constant-arrival-rate',
    rate: containsRequestsPerSecond,
    timeUnit: '1s',
    duration: `${durationS}s`,
    preAllocatedVUs: 10,
    maxVUs: 10000,
    exec: 'onlyContains',
    gracefulStop: '20s'
}

const removeScenario = {
    executor: 'constant-arrival-rate',
    rate: removeRequestsPerSecond,
    timeUnit: '1s',
    duration: `${durationS}s`,
    preAllocatedVUs: 10,
    maxVUs: 3000,
    exec: 'onlyRemove',
    gracefulStop: '20s'
}

export let options = {
    scenarios: { }
}

if (containsRequestsPerSecond > 0) {
    options.scenarios['contains'] = containsScenario
}

if (removeRequestsPerSecond > 0) {
    options.scenarios['remove'] = removeScenario
}

if (addRequestsPerSecond > 0) {
    options.scenarios['add'] = addScenario
}

function randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min)
}

function count() {
    const result = http.get(`http://${ip}/${kind}/count`, params)
    recordsCount.add(result.body)
}

function add(courseId, studentId) {
    return http.post(`http://${ip}/${kind}?studentId=${studentId}&courseId=${courseId}`, params)
}

function contains(courseId, studentId) {
    return http.get(`http://${ip}/${kind}?studentId=${studentId}&courseId=${courseId}`, params)
}

function remove(courseId, studentId) {
    return http.del(`http://${ip}/${kind}?studentId=${studentId}&courseId=${courseId}`, params)
}

export function setup() {
    for (let i = 0; i < 100; i++) {
        const id = randomInt(0, maxRecordsCount)
        add(id, id)
        console.log(id)
        count()
    }
}

export function onlyAdd() {
    const id = randomInt(0, maxRecordsCount)
    const res = add(id, id)
    count()
    addCount.add(1)
    addDuration.add(res.timings.duration)
}

export function onlyContains() {
    const id = randomInt(0, maxRecordsCount)
    const res = contains(id, id)
    containsCount.add(1)
    containsDuration.add(res.timings.duration)
}

export function onlyRemove() {
    const id = randomInt(0, maxRecordsCount)
    const res = remove(id, id)
    count()
    removeCount.add(1)
    removeDuration.add(res.timings.duration)
}
