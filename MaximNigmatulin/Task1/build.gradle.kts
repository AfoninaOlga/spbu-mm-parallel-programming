import java.io.File.separatorChar as sep

plugins {
    kotlin("jvm") version "1.8.0"
    application
}

repositories {
    mavenCentral()
}

val appMainClass = "hyper_quicksort.app.MainKt"

// TODO: fix hardcoded value
// val mpjHome = System.getenv("MPJ_HOME") ?: error("Specify `MPJ_HOME` environment variable")
val mpjHome = "/Users/emax/Data/mpj"
val mpjStarterJar = files("$mpjHome${sep}lib${sep}starter.jar")
val mpjJar = files("$mpjHome${sep}lib${sep}mpj.jar")
val mpjClassPath = sourceSets.main.get().runtimeClasspath - mpjJar


// TODO: change positional args to string args
val (numberOfProcesses, inputFilename, outputFilename) = (project.properties["args"] as? String? ?: "")
    .split(" ")
    .filterNot { it.isBlank() }
    .let {
        Triple(
            it.getOrElse(0) { "1" },
            listOfNotNull(it.getOrNull(1)),
            listOfNotNull(it.getOrNull(2))
        )
    }

dependencies {
    implementation(mpjJar)
}

java {
    toolchain {
        languageVersion.set(JavaLanguageVersion.of(8))
    }
}

tasks.withType<JavaExec> {
    mainClass.set("runtime.starter.MPJRun")
    classpath = mpjStarterJar
    args = listOf(appMainClass) +
            listOf("-cp", mpjClassPath.asPath) +
            listOf("-np", numberOfProcesses) +
            inputFilename +
            outputFilename

    dependsOn("classes")
}
