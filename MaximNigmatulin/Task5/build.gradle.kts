plugins {
    kotlin("jvm") version "1.8.21"
    id("org.jetbrains.kotlinx.kover") version "0.6.1"
    id("org.openjfx.javafxplugin") version "0.0.14"
    application
}

group = "app.chat"
version = "1.0-SNAPSHOT"

repositories {
    mavenCentral()
}

dependencies {
    testImplementation(kotlin("test"))
    implementation(group="com.google.code.gson", name="gson", version="2.10")
    testImplementation("org.junit.jupiter:junit-jupiter-params")
}

tasks.test {
    useJUnitPlatform()
}

kotlin {
    jvmToolchain(11)
}

application {
    mainClass.set("ChatApp")
}

javafx {
    modules("javafx.controls", "javafx.fxml")
}
