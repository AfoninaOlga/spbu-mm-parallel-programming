package spbu.plugins

import io.ktor.server.routing.*
import io.ktor.server.application.*
import spbu.routes.recordRouting

fun Application.configureRouting() {
    routing {
        recordRouting()
    }
}
