plugins {
    kotlin("jvm") version "1.8.0"
    id("org.jetbrains.kotlinx.kover") version "0.6.1"
    application
}

group = "org.example"
version = "1.0-SNAPSHOT"

tasks.named<JavaExec>("run") {
    standardInput = System.`in`
}

repositories {
    mavenCentral()
}

dependencies {
    testImplementation(kotlin("test"))
}

tasks.test {
    useJUnitPlatform()
}

kotlin {
    jvmToolchain(17)
}

application {
    mainClass.set("MainKt")
}
