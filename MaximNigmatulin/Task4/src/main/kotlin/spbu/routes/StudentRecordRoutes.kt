package spbu.routes

import io.ktor.http.*
import io.ktor.server.application.*
import io.ktor.server.response.*
import io.ktor.server.routing.*
import io.ktor.server.util.*
import spbu.models.StudentRecord
import spbu.system.ExamSystem
import spbu.system.ExamSystemInterface
import spbu.system.fineGrainedStorage
import spbu.system.lazyStorage

fun validateRootRoute(queryParam: String): ExamSystem {
    if (queryParam == ExamSystem.FINE_GRAINED.value) {
        return ExamSystem.FINE_GRAINED
    }
    if (queryParam == ExamSystem.LAZY.value) {
        return ExamSystem.LAZY
    }
    throw IllegalArgumentException("This exam system is not defined!")
}

fun getStorage(es: ExamSystem): ExamSystemInterface {
    if (es == ExamSystem.FINE_GRAINED) {
        return fineGrainedStorage
    }
    if (es == ExamSystem.LAZY) {
        return lazyStorage
    }
    throw IllegalArgumentException("This exam system is not defined!")
}

fun getValidateStorage(queryParam: String): ExamSystemInterface {
    val storageType = validateRootRoute(queryParam)
    return getStorage(storageType)
}

fun Route.recordRouting() {
    route("/{exam_system}") {
        route("/count") {
            get {
                val examSystemParam = call.parameters.getOrFail("exam_system")
                call.respond(HttpStatusCode.OK, getValidateStorage(examSystemParam).count())
            }
        }
        get {
            val examSystemParam = call.parameters.getOrFail("exam_system")
            val queryStudentId = call.request.queryParameters["studentId"]?.toLong() ?: return@get call.respond(
                HttpStatusCode.BadRequest
            )
            val queryCourseId =
                call.request.queryParameters["courseId"]?.toLong() ?: return@get call.respond(HttpStatusCode.BadRequest)

            if (getValidateStorage(examSystemParam).contains(StudentRecord(queryStudentId, queryCourseId))) {
                call.respond(HttpStatusCode.OK, "($queryStudentId, $queryCourseId) in collection\n")
            } else {
                call.respond(HttpStatusCode.OK, "($queryStudentId, $queryCourseId) not in collection\n")
            }
        }
        post {
            val examSystemParam = call.parameters.getOrFail("exam_system")
            val queryStudentId = call.request.queryParameters["studentId"]?.toLong() ?: return@post call.respond(
                HttpStatusCode.BadRequest
            )
            val queryCourseId =
                call.request.queryParameters["courseId"]?.toLong()
                    ?: return@post call.respond(HttpStatusCode.BadRequest)

            if (getValidateStorage(examSystemParam).add(StudentRecord(queryStudentId, queryCourseId))) {
                call.respond(HttpStatusCode.OK, "($queryStudentId, $queryCourseId) added\n")
            } else {
                call.respond(HttpStatusCode.OK, "($queryStudentId, $queryCourseId) already in collection\n")
            }
        }

        delete() {
            val examSystemParam = call.parameters.getOrFail("exam_system")
            val queryStudentId = call.request.queryParameters["studentId"]?.toLong() ?: return@delete call.respond(
                HttpStatusCode.BadRequest
            )
            val queryCourseId =
                call.request.queryParameters["courseId"]?.toLong()
                    ?: return@delete call.respond(HttpStatusCode.BadRequest)

            if (getValidateStorage(examSystemParam).remove(StudentRecord(queryStudentId, queryCourseId))) {
                call.respond(HttpStatusCode.OK, "($queryStudentId, $queryCourseId) removed\n")
            } else {
                call.respond(HttpStatusCode.OK, "($queryStudentId, $queryCourseId) not in collection\n")
            }
        }
    }
}
