import { useEffect, useMemo, useState } from 'react'
import './App.css'

type SpeakerSummary = {
  id: number
  name: string
  title: string
  company: string
}

type SpeakerDetails = SpeakerSummary & {
  bio: string
  photoUrl: string
}

type Session = {
  id: number
  title: string
  summary: string
  description: string
  track: string
  level: string
  room: string
  startTimeUtc: string
  endTimeUtc: string
  speaker: SpeakerSummary
}

type ViewMode = 'sessions' | 'favorites'

function App() {
  const [sessions, setSessions] = useState<Session[]>([])
  const [speakers, setSpeakers] = useState<SpeakerDetails[]>([])
  const [selectedSessionId, setSelectedSessionId] = useState<number | null>(null)
  const [favoriteSessionIds, setFavoriteSessionIds] = useState<number[]>([])
  const [viewMode, setViewMode] = useState<ViewMode>('sessions')
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [sessionsResponse, speakersResponse] = await Promise.all([
          fetch('/api/sessions'),
          fetch('/api/speakers'),
        ])

        if (!sessionsResponse.ok || !speakersResponse.ok) {
          throw new Error('Unable to load conference data')
        }

        const sessionsData = (await sessionsResponse.json()) as Session[]
        const speakersData = (await speakersResponse.json()) as SpeakerDetails[]

        setSessions(sessionsData)
        setSpeakers(speakersData)
        setSelectedSessionId(sessionsData[0]?.id ?? null)
      } catch (error) {
        console.error(error)
      } finally {
        setLoading(false)
      }
    }

    void fetchData()
  }, [])

  const favoriteSessions = useMemo(
    () => sessions.filter((session) => favoriteSessionIds.includes(session.id)),
    [favoriteSessionIds, sessions],
  )

  const visibleSessions = viewMode === 'favorites' ? favoriteSessions : sessions

  const selectedSession = useMemo(() => {
    const source = visibleSessions.length > 0 ? visibleSessions : sessions
    return source.find((session) => session.id === selectedSessionId) ?? source[0]
  }, [selectedSessionId, sessions, visibleSessions])

  const selectedSpeaker = useMemo(
    () => speakers.find((speaker) => speaker.name === selectedSession?.speaker.name) ?? speakers[0],
    [selectedSession, speakers],
  )

  const isFavorite = selectedSession ? favoriteSessionIds.includes(selectedSession.id) : false

  const toggleFavorite = () => {
    if (!selectedSession) {
      return
    }

    setFavoriteSessionIds((currentFavoriteIds) => {
      if (currentFavoriteIds.includes(selectedSession.id)) {
        return currentFavoriteIds.filter((favoriteId) => favoriteId !== selectedSession.id)
      }

      return [...currentFavoriteIds, selectedSession.id]
    })
  }

  return (
    <div className="app-shell">
      <header className="topbar">
        <div>
          <p className="eyebrow">Conference planner</p>
          <h1>SessionHub</h1>
        </div>

        <div className="topbar-actions">
          <nav className="topbar-nav" aria-label="Primary navigation">
            <button
              type="button"
              className={`nav-item ${viewMode === 'sessions' ? 'active' : ''}`}
              onClick={() => setViewMode('sessions')}
            >
              Sessions
            </button>
            <button
              type="button"
              className={`nav-item ${viewMode === 'favorites' ? 'active' : ''}`}
              onClick={() => setViewMode('favorites')}
            >
              Favorites
            </button>
          </nav>
          <div className="header-pill">
            {viewMode === 'favorites' ? `${favoriteSessions.length} favorites` : `${sessions.length} sessions • ${speakers.length} speakers`}
          </div>
        </div>
      </header>

      <main className="layout">
        <aside className="panel session-list-panel">
          <div className="panel-header">
            <h2>{viewMode === 'favorites' ? 'Favorites' : 'Sessions'}</h2>
            <span>{viewMode === 'favorites' ? favoriteSessions.length : sessions.length}</span>
          </div>

          {loading ? (
            <p className="empty-state">Loading sessions…</p>
          ) : visibleSessions.length === 0 ? (
            <p className="empty-state">
              {viewMode === 'favorites' ? 'No favorites saved yet.' : 'No sessions available right now.'}
            </p>
          ) : (
            <ul className="session-list">
              {visibleSessions.map((session) => (
                <li key={session.id}>
                  <button
                    type="button"
                    className={`session-item ${selectedSession?.id === session.id ? 'selected' : ''}`}
                    onClick={() => setSelectedSessionId(session.id)}
                  >
                    <div className="session-meta-row">
                      <span className="badge track">{session.track}</span>
                      <span className="badge level">{session.level}</span>
                    </div>
                    <h3>{session.title}</h3>
                    <p>{session.speaker.name}</p>
                    <div className="session-meta-row small-row">
                      <span>{new Date(session.startTimeUtc).toLocaleString([], { month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' })}</span>
                      <span>{session.room}</span>
                    </div>
                  </button>
                </li>
              ))}
            </ul>
          )}
        </aside>

        <section className="panel session-detail-panel">
          {selectedSession ? (
            <>
              <div className="detail-header">
                <div>
                  <p className="eyebrow">{selectedSession.track}</p>
                  <h2>{selectedSession.title}</h2>
                </div>
                <div className="detail-actions">
                  <div className="detail-tags">
                    <span className="badge dark">{selectedSession.level}</span>
                    <span className="badge dark">{selectedSession.room}</span>
                  </div>
                  <button
                    type="button"
                    className={`favorite-toggle ${isFavorite ? 'active' : ''}`}
                    onClick={toggleFavorite}
                    aria-pressed={isFavorite}
                  >
                    {isFavorite ? '★ Favorited' : '☆ Favorite'}
                  </button>
                </div>
              </div>

              <p className="summary">{selectedSession.summary}</p>

              <div className="meta-grid">
                <div>
                  <label>Speaker</label>
                  <strong>{selectedSession.speaker.name}</strong>
                </div>
                <div>
                  <label>Time</label>
                  <strong>
                    {new Date(selectedSession.startTimeUtc).toLocaleString([], {
                      month: 'short',
                      day: 'numeric',
                      hour: 'numeric',
                      minute: '2-digit',
                    })}{' '}
                    -{' '}
                    {new Date(selectedSession.endTimeUtc).toLocaleString([], {
                      month: 'short',
                      day: 'numeric',
                      hour: 'numeric',
                      minute: '2-digit',
                    })}
                  </strong>
                </div>
                <div>
                  <label>Track</label>
                  <strong>{selectedSession.track}</strong>
                </div>
              </div>

              <div className="description-block">
                <h3>About this session</h3>
                <p>{selectedSession.description}</p>
              </div>

              <div className="speaker-card">
                <div className="speaker-header">
                  <div>
                    <p className="eyebrow">Featured speaker</p>
                    <h3>{selectedSpeaker?.name ?? selectedSession.speaker.name}</h3>
                    <p className="speaker-role">{selectedSpeaker?.title ?? selectedSession.speaker.title}</p>
                    <p className="speaker-company">{selectedSpeaker?.company ?? selectedSession.speaker.company}</p>
                  </div>
                </div>
                <p className="speaker-bio">{selectedSpeaker?.bio ?? 'Speaker bio is available in the API response.'}</p>
              </div>
            </>
          ) : (
            <p className="empty-state">Select a session to view details.</p>
          )}
        </section>
      </main>
    </div>
  )
}

export default App
