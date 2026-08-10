let currentProvider = null;

async function loadMyPage()
{
    try
    {
        const response = await fetch("https://localhost:7226/api/User/mypage",
            {
                method: "GET",
                credentials: "include"
            });
        if (response.status === 401)
        {

            alert("로그인이 필요합니다.");
            window.location.href = "/accounts/login";

            return;
        }
        if (!response.ok)
        {

            throw new Error("마이 페이지 정보를 가져오지 못했습니다.");
        }
        const data = await response.json();
        currentProvider = data.provider ?? null;

        const gender = data.gender === "M" ? "남성" : data.gender === "F" ? "여성" : "-";

        // 프로필
        document.getElementById("avatarText").textContent    = (data.userName || "U").charAt(0);
        document.getElementById("userName").textContent      = data.userName ?? "-";
        document.getElementById("email").textContent         = data.email ?? "-";
        document.getElementById("userId").textContent        = data.userId ?? "-";

        // 회원 정보
        document.getElementById("infoUserName").textContent  = data.userName ?? "-";
        document.getElementById("infoEmail").textContent     = data.email ?? "-";
        document.getElementById("phoneNumber").textContent   = data.phoneNumber ?? "-";
        document.getElementById("gender").textContent        = gender;
        document.getElementById("birthDate").textContent     = formatDate(data.birthDate);

        document.getElementById("zipCode").textContent       = data.zipCode ?? "-";
        document.getElementById("baseAddress").textContent   = data.baseAddress ?? "-";
        document.getElementById("detailAddress").textContent = data.detailAddress ? ` ${data.detailAddress}` : "";
        document.getElementById("createdAt").textContent     = formatDate(data.createdAt);

        // 자체 로그인 / 소셜 로그인
        document.getElementById("provider").textContent      = data.provider ?? "자체 로그인";


        const emailVerified     = document.getElementById("emailVerified");
        const emailVerifyButton = document.getElementById("emailVerifyButton");

        if (data.emailVerified)
        {
            emailVerified.textContent = "✓ 인증 완료";
            emailVerified.classList.add("verified");
            emailVerifyButton.style.display = "none";
        }
        else
        {
            emailVerified.textContent = "미인증";
            emailVerified.classList.remove("verified");
            emailVerifyButton.style.display = "inline-block";
        }

        console.log(data);
        console.log("Provider : ", data.provider);

    } catch (error)
    {
        console.error(error);
        alert("마이페이지 정보를 불러오는 중 오류가 발생했습니다.");
    }
    
}

function formatDate(value)
{
    if (!value)
    {
        return "-";
    }
    return value.substring(0, 10);
}

function goToUserUpdate()
{
    window.location.href = "/accounts/user-update";
}

function goToPasswordChange() 
{
    if (currentProvider)
    {
        alert(`${currentProvider} 소셜 로그인 계정은 비밀번호 변경을 지원하지 않습니다.`);
        return;
    }
    window.location.href = "/accounts/password-change";
}

document.addEventListener("DOMContentLoaded", loadMyPage);



const emailVerifyButton        = document.getElementById("emailVerifyButton");
const emailVerifyModal         = document.getElementById("emailVerifyModal");
const emailVerifyConfirmButton = document.getElementById("emailVerifyConfirmButton");
const emailVerifyCloseButton   = document.getElementById("emailVerifyCloseButton");
const verificationCodeInput    = document.getElementById("verificationCode");

const verifyEmailAddress       = document.getElementById("verifyEmailAddress");
// ============================================================
// 인증번호 발송 
// ============================================================

emailVerifyButton.addEventListener("click", async () => {

    emailVerifyButton.disabled = true;
    emailVerifyButton.textContent = "전송 중...";

    try
    {
        const response = await fetch("https://localhost:7226/api/User/email-verification", {
            method: "POST",
            credentials: "include"
            }
        );

        const result = await response.json();

        if (!response.ok)
        {
            alert(result.retMsg ?? "인증번호 전송에 실패했습니다.");

            return;
        }

        if (result.retVal !== 0)
        {
            alert(result.retMsg);

            return;
        }

        alert("인증번호가 전송되었습니다.");

        verifyEmailAddress.textContent = document.getElementById("infoEmail").textContent; // 모달에 현재 이메일 표시        
        verificationCodeInput.value = "";                                                  // 인증번호 입력창 초기화        
        emailVerifyModal.classList.add("active");                                          // 모달 열기        
        verificationCodeInput.focus();                                                     // 입력창에 포커스
    }
    catch (error)
    {
        console.error("인증번호 전송 오류:", error);
        alert("서버와 통신하는 중 오류가 발생했습니다.");
    }
    finally
    {
        emailVerifyButton.disabled = false; emailVerifyButton.textContent = "이메일 인증";
    }
});

// ============================================================
// 인증번호 확인
// ============================================================

emailVerifyConfirmButton.addEventListener("click", async () => {

    const verificationCode = verificationCodeInput.value.trim();
    const email            = document.getElementById("infoEmail").textContent.trim();

    // 인증번호 입력 여부
    if (!verificationCode)
    {
        alert("인증번호를 입력해주세요.");
        verificationCodeInput.focus();

        return;
    }
    if (!/^\d{6}$/.test(verificationCode))
    {
        alert("인증번호 6자리를 입력해주세요.");
        verificationCodeInput.focus();

        return;
    }
    emailVerifyConfirmButton.disabled    = true;
    emailVerifyConfirmButton.textContent = "확인 중...";

    try {
        const response = await fetch("https://localhost:7226/api/User/email-verification/verify",
            {
                method: "POST",
                headers:
                {
                    "Content-Type": "application/json"
                },
                credentials: "include",
                body: JSON.stringify(
                    {
                        email: email,
                        verificationCode: verificationCode
                    }
                )
            }
        );
        const result = await response.json();

        if (!response.ok)
        {
            alert(result.retMsg ?? "이메일 인증에 실패했습니다.");

            return;
        }

        if (result.retVal !== 0)
        {
            alert(result.retMsg);

            return;
        }
        // 인증 성공
        alert(result.retMsg);

        const emailVerified = document.getElementById("emailVerified");

        emailVerified.textContent = "✓ 인증 완료";
        emailVerified.classList.add("verified");
        emailVerifyButton.style.display = "none";

        // 모달 닫기
        emailVerifyModal.classList.remove("active");
    }
    catch (error)
    {
        console.error("이메일 인증 오류:", error);

        alert("서버와 통신하는 중 오류가 발생했습니다.");
    }
    finally
    {
        emailVerifyConfirmButton.disabled    = false;
        emailVerifyConfirmButton.textContent = "인증하기";
    }
});

// ============================================================
// 모달 닫기
// ============================================================
emailVerifyCloseButton.addEventListener("click", () => {
    emailVerifyModal.classList.remove("active");
});